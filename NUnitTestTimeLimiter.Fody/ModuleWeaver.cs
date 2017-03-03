using JetBrains.Annotations;
using Mono.Cecil;
using NUnitTestTimeLimiter.Fody;
using System;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil.Rocks;

// ReSharper disable once CheckNamespace => This is the convention suggested by the BasicFodyAddin Fody template.
public class ModuleWeaver
{
    private const int DefaultTimeLimit = 2000;
    private static int _timeLimit = DefaultTimeLimit;

    // ReSharper disable once MemberCanBePrivate.Global => Fody will use this when calling our module.
    // ReSharper disable once UnusedAutoPropertyAccessor.Global => Fody will use this when calling our module.
    public XElement Config { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global => Fody will use this when calling our module.
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global => Fody will use this when calling our module.
    [NotNull]
    public Action<string> LogInfo { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global => Fody will use this when calling our module.
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global => Fody will use this when calling our module.
    [NotNull]
    public Action<string> LogWarning { get; set; }

    // An instance of Mono.Cecil.ModuleDefinition for processing
    // ReSharper disable once MemberCanBePrivate.Global => Fody will use this when calling our module.
    // ReSharper disable once UnusedAutoPropertyAccessor.Global => Fody will use this when calling our module.
    public ModuleDefinition ModuleDefinition { get; set; }

    // Init logging delegates to make testing easier
    public ModuleWeaver()
    {
        LogInfo = msg => { };
        LogWarning = msg => { };
    }

    private static void AddTimeoutAttribute(
        [NotNull] ModuleDefinition moduleDefinition,
        [NotNull] ICustomAttributeProvider typeDefinition,
        [NotNull] TypeDefinition timeoutAttribute)
    {
        var attributeConstructorReference = timeoutAttribute
            .GetConstructors()?
            .Where(ctor => ctor?.Parameters != null)
            .FirstOrDefault(
                ctor =>
                    ctor.Parameters.Count == 1 &&
                    TypeReferenceExtensions.EqualsFullName(ctor.Parameters[0]?.ParameterType, moduleDefinition.TypeSystem?.Int32));
        var attributeConstructor = moduleDefinition.ImportReference(attributeConstructorReference);
        var attribute = new CustomAttribute(attributeConstructor);
        attribute.ConstructorArguments?.Add(new CustomAttributeArgument(moduleDefinition.TypeSystem?.Int32, _timeLimit));
        typeDefinition.CustomAttributes?.Add(attribute);
    }

    [NotNull]
    private ModuleDefinition CheckIfModuleDefinitionIsSet()
    {
        if (ModuleDefinition == null)
        {
            throw new InvalidOperationException($"{nameof(ModuleDefinition)} == null");
        }

        return ModuleDefinition;
    }

    private static CustomAttributeArgument CreateMaxTimeLimitArgument([NotNull] ModuleDefinition moduleDefinition)
    {
        return new CustomAttributeArgument(moduleDefinition.TypeSystem?.Int32, _timeLimit);
    }

    private void ParseConfiguration()
    {
        var configurationAttributes = Config?.Attributes() ?? Enumerable.Empty<XAttribute>();
        foreach (var a in configurationAttributes.Where(a => a != null))
        {
            ParseTimeLimitConfiguration(a);
        }
    }

    private static void ParseTimeLimitConfiguration([NotNull] XAttribute a)
    {
        int timeLimit;
        if (a.Name == "TimeLimit" && int.TryParse(a.Value, out timeLimit))
        {
            _timeLimit = timeLimit;
        }
    }

    private static void SetTimeout(
        [NotNull] ModuleDefinition moduleDefinition,
        TypeDefinition typeDefinition,
        [NotNull] TypeDefinition timeoutAttribute)
    {
        if (typeDefinition == null)
        {
            return;
        }

        var hasTimeout = typeDefinition.HasAttribute(timeoutAttribute);
        if (hasTimeout)
        {
            VerifyAndAdjustTimeoutAttribute(moduleDefinition, typeDefinition, timeoutAttribute);
            return;
        }

        AddTimeoutAttribute(moduleDefinition, typeDefinition, timeoutAttribute);
    }

    private static void VerifyAndAdjustTimeoutAttribute(
        [NotNull] ModuleDefinition moduleDefinition,
        [NotNull] TypeDefinition typeDefinition,
        [NotNull] TypeDefinition timeoutAttribute)
    {
        var attritube = typeDefinition.Attribute(timeoutAttribute);
        var maxTimeLimitArgument = CreateMaxTimeLimitArgument(moduleDefinition);
        if (attritube?.ConstructorArguments == null)
        {
            return;
        }

        if (attritube.ConstructorArguments.Count == 0)
        {
            attritube.ConstructorArguments.Add(maxTimeLimitArgument);
            return;
        }

        var constructorArgument = attritube.ConstructorArguments[0];
        var intValue = (int?)constructorArgument.Value;
        if (!intValue.HasValue || intValue.Value <= _timeLimit)
        {
            return;
        }

        attritube.ConstructorArguments.Clear();
        attritube.ConstructorArguments.Add(maxTimeLimitArgument);
    }

    public void Execute()
    {
        try
        {
            ParseConfiguration();
            ModuleConstants.LogWarning = LogWarning;



            var moduleDefinition = CheckIfModuleDefinitionIsSet();
            var nunitDefinition = new NUnitDefinition(moduleDefinition);
            if (!nunitDefinition.NUnitPresent || nunitDefinition.TimeoutAttribute == null)
            {
                LogWarning(
                    $"No NUnit reference in the assembly, exiting {ModuleConstants.ModuleName} (this assembly should not have this Fody module installed).");
                return;
            }

            var assemblyDefinition = ModuleDefinition.AssemblyDefinition();
            var moduleDefinitions = assemblyDefinition.ModuleDefinitions();
            var types = moduleDefinitions.TypeDefinitionsWithAttribute(nunitDefinition.TestFixtureAttribute);
            types.ToList().ForEach(td => SetTimeout(moduleDefinition, td, nunitDefinition.TimeoutAttribute));
        }
        catch (Exception ex)
        {
            LogInfo($"Caught exception: {ex.Message}, {ModuleConstants.ModuleName} weawing aborted!");
            throw;
        }
    }
}