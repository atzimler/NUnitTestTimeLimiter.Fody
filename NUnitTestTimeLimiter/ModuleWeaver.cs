using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnitTestTimeLimiter.Fody;
using MethodAttributes = Mono.Cecil.MethodAttributes;

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

    public void Execute()
    {
        try
        {
            var testFixtureAttribute = typeof(TestFixtureAttribute).TypeReference(ModuleDefinition);

            var assemblyDefinition = ModuleDefinition.GetAssemblyDefinition();
            var moduleDefinitions = assemblyDefinition.GetModuleDefinitions();
            var types = moduleDefinitions.GetTypeDefinitionsWithAttribute(testFixtureAttribute);
            foreach (var type in types)
            {
                LogInfo(type.FullName);
            }
        }
        catch (Exception ex)
        {
            LogInfo($"Caught exception: {ex.Message}, {ModuleConstants.ModuleName} weawing aborted!");
        }
        //if (ModuleDefinition == null)
        //{
        //    LogInfo("ModuleDefinition == null, aborting!");
        //    return;
        //}

        //var assembly = ModuleDefinition.Assembly;

        //_typeSystem = ModuleDefinition.TypeSystem;


        //var newType = new TypeDefinition(null, "Hello", TypeAttributes.Public, _typeSystem.Object);

        //AddConstructor(newType);

        //AddHelloWorld(newType);

        //ModuleDefinition.Types.Add(newType);
        //LogInfo("Added type 'Hello' with method 'World'.");
    }


    //void AddConstructor(TypeDefinition newType)
    //{
    //    var method = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, _typeSystem.Void);
    //    var objectConstructor = ModuleDefinition.Import(_typeSystem.Object.Resolve().GetConstructors().First());
    //    var processor = method.Body.GetILProcessor();
    //    processor.Emit(OpCodes.Ldarg_0);
    //    processor.Emit(OpCodes.Call, objectConstructor);
    //    processor.Emit(OpCodes.Ret);
    //    newType.Methods.Add(method);
    //}

    //void AddHelloWorld(TypeDefinition newType)
    //{
    //    var method = new MethodDefinition("World", MethodAttributes.Public, _typeSystem.String);
    //    var processor = method.Body.GetILProcessor();
    //    processor.Emit(OpCodes.Ldstr, "Hello World");
    //    processor.Emit(OpCodes.Ret);
    //    newType.Methods.Add(method);
    //}
}