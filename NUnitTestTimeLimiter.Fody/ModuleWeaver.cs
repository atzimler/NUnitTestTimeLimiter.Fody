using JetBrains.Annotations;
using Mono.Cecil;
using NUnitTestTimeLimiter.Fody;
using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil.Rocks;
using MoreLinq;

// ReSharper disable once CheckNamespace => This is the convention suggested by the BasicFodyAddin Fody template.
public class ModuleWeaver
{
    // FIXME: The tests also use this information, so there is a feature missing, which in turn also has its tests missing.
    // FIXME: Make these private again.
    public const string NUnitFrameworkAssembly = "nunit.framework";
    public const string NUnitFrameworkNamespace = "NUnit.Framework";
    public const string TimoutAttribute = "TimeoutAttribute";
    public const string TestFixtureAttribute = "TestFixtureAttribute";


    private const int DefaultTimeLimit = 2000;
    private static int _timeLimit = DefaultTimeLimit;

    public XElement Config { get; set; }

    // Will log an informational message to MSBuild
    // ReSharper disable once MemberCanBePrivate.Global => Fody will use this when calling our module.
    [NotNull]
    public Action<string> LogInfo { get; }

    // An instance of Mono.Cecil.ModuleDefinition for processing
    // ReSharper disable once MemberCanBePrivate.Global => Fody will use this when calling our module.
    // ReSharper disable once UnusedAutoPropertyAccessor.Global => Fody will use this when calling our module.
    public ModuleDefinition ModuleDefinition { get; set; }

    // Init logging delegates to make testing easier
    public ModuleWeaver()
    {
        LogInfo = msg =>
        {
            if (msg != null)
            {
                Debug.WriteLine($"{ModuleConstants.ModuleName}: {msg}");
            }
        };
    }

    private static void AddTimeoutAttribute(
        [NotNull] ModuleDefinition moduleDefinition,
        [NotNull] ICustomAttributeProvider typeDefinition,
        [NotNull] TypeDefinition timeoutAttribute)
    {
        var attributeConstructor = moduleDefinition.ImportReference(timeoutAttribute.GetConstructors().First(ctor => ctor.Parameters.Count == 1));
        //var attributeConstructor =
        //    moduleDefinition.ImportReference(timeoutAttribute.GetConstructors(new[] { typeof(int) }));
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

    private static TypeDefinition LoadTypeDefinition(
        [NotNull] AssemblyDefinition assemblyDefinition,
        [NotNull] string @namespace,
        [NotNull] string name
    )
    {
        var typeDefinition = assemblyDefinition.MainModule?.GetType(@namespace, name);
        if (typeDefinition == null)
        {
            throw new TypeLoadException($"Unable to load type {@namespace}.{name} from assembly {assemblyDefinition.FullName}");
        }

        return typeDefinition;
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
        var maxTimeLimitArgument = new CustomAttributeArgument(moduleDefinition.TypeSystem?.Int32, _timeLimit);
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
            //var configurationAttributes = Config.Attributes();
            //foreach (var a in configurationAttributes)
            //{
            //    int timeLimit;
            //    if (a.Name == "TimeLimit" && int.TryParse(a.Value, out timeLimit))
            //    {
            //        _timeLimit = timeLimit;
            //    }
            //}

            var moduleDefinition = CheckIfModuleDefinitionIsSet();
            var nunitAssembly = moduleDefinition.ReferencedAssembly(NUnitFrameworkAssembly);

            var timeoutAttribute = LoadTypeDefinition(nunitAssembly, NUnitFrameworkNamespace, TimoutAttribute);
            var testFixtureAttribute = LoadTypeDefinition(nunitAssembly, NUnitFrameworkNamespace, TestFixtureAttribute);



            var assemblyDefinition = ModuleDefinition.AssemblyDefinition();
            var moduleDefinitions = assemblyDefinition.ModuleDefinitions();
            var types = moduleDefinitions.TypeDefinitionsWithAttribute(testFixtureAttribute);
            types.ForEach(td => SetTimeout(moduleDefinition, td, timeoutAttribute));
        }
        catch (Exception ex)
        {
            LogInfo($"Caught exception: {ex.Message}, {ModuleConstants.ModuleName} weawing aborted!");
            throw;
        }
    }
}