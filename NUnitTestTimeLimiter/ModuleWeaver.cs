using JetBrains.Annotations;
using Mono.Cecil;
using NUnit.Framework;
using NUnitTestTimeLimiter.Fody;
using System;
using System.Diagnostics;
using System.Linq;

// ReSharper disable once CheckNamespace => This is the convention suggested by the BasicFodyAddin Fody template.
public class ModuleWeaver
{
    // Will log an informational message to MSBuild
    // ReSharper disable once MemberCanBePrivate.Global => Fody will probably use this when calling our module.
    [NotNull]
    public Action<string> LogInfo { get; set; }

    // An instance of Mono.Cecil.ModuleDefinition for processing
    // ReSharper disable once MemberCanBePrivate.Global => Fody will probably use this when calling our module.
    // ReSharper disable once UnusedAutoPropertyAccessor.Global => Fody will probably use this when calling our module.
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

    private void SetTimeout([NotNull] TypeDefinition typeDefinition)
    {
        if (typeDefinition.CustomAttributes.Any(ca => ca.AttributeType.FullName == typeof(TimeoutAttribute).FullName))
        {
            return;
        }

        var attributeConstructor =
            ModuleDefinition.ImportReference(typeof(TimeoutAttribute).GetConstructor(new[] { typeof(int) }));
        var attribute = new CustomAttribute(attributeConstructor);
        attribute.ConstructorArguments.Add(new CustomAttributeArgument(ModuleDefinition.TypeSystem.Int32, 2000));
        typeDefinition.CustomAttributes.Add(attribute);
    }

    public void Execute()
    {
        try
        {
            if (ModuleDefinition == null)
            {
                throw new InvalidOperationException($"{nameof(ModuleDefinition)} == null");
            }

            var testFixtureAttribute = ModuleDefinition.ImportReference(typeof(TestFixtureAttribute));

            var assemblyDefinition = ModuleDefinition.AssemblyDefinition();
            var moduleDefinitions = assemblyDefinition.ModuleDefinitions();
            var types = moduleDefinitions.TypeDefinitionsWithAttribute(testFixtureAttribute);
            types.ToList().ForEach(SetTimeout);
        }
        catch (Exception ex)
        {
            LogInfo($"Caught exception: {ex.Message}, {ModuleConstants.ModuleName} weawing aborted!");
        }
    }
}