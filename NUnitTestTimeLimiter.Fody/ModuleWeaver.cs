﻿using JetBrains.Annotations;
using Mono.Cecil;
using NUnit.Framework;
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
    private const string NUnitFramework = "NUnit.Framework";
    private const string TimoutAttribute = "TimeoutAttribute";
    private const string TestFixtureAttribute = "TestFixtureAttribute";


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

    private static void AddTimeoutAttribute([NotNull] ModuleDefinition moduleDefinition, [NotNull] ICustomAttributeProvider typeDefinition)
    {
        var attributeConstructor =
            moduleDefinition.ImportReference(typeof(TimeoutAttribute).GetConstructor(new[] { typeof(int) }));
        var attribute = new CustomAttribute(attributeConstructor);
        attribute.ConstructorArguments?.Add(new CustomAttributeArgument(moduleDefinition.TypeSystem?.Int32, _timeLimit));
        typeDefinition.CustomAttributes?.Add(attribute);
    }

    private static void SetTimeout([NotNull] ModuleDefinition moduleDefinition, TypeDefinition typeDefinition)
    {
        if (typeDefinition == null)
        {
            return;
        }

        var timeoutAttribute = moduleDefinition.ImportReference(typeof(TimeoutAttribute));
        var hasTimeout = typeDefinition.HasAttribute(timeoutAttribute);
        if (hasTimeout)
        {
            VerifyAndAdjustTimeoutAttribute(moduleDefinition, typeDefinition);
            return;
        }

        AddTimeoutAttribute(moduleDefinition, typeDefinition);
    }

    private static void VerifyAndAdjustTimeoutAttribute([NotNull] ModuleDefinition moduleDefinition,
        [NotNull] TypeDefinition typeDefinition)
    {
        var timeoutAttribute = moduleDefinition.ImportReference(typeof(TimeoutAttribute));
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

            var moduleDefinition = ModuleDefinition;
            if (moduleDefinition == null)
            {
                throw new InvalidOperationException($"{nameof(ModuleDefinition)} == null");
            }

            //var nunitAssembly = moduleDefinition.ReferencedAssembly("nunit.framework");

            var testFixtureAttribute = moduleDefinition.ImportReference(typeof(TestFixtureAttribute));

            var assemblyDefinition = ModuleDefinition.AssemblyDefinition();
            var moduleDefinitions = assemblyDefinition.ModuleDefinitions();
            var types = moduleDefinitions.TypeDefinitionsWithAttribute(testFixtureAttribute);
            types.ForEach(td => SetTimeout(moduleDefinition, td));
        }
        catch (Exception ex)
        {
            LogInfo($"Caught exception: {ex.Message}, {ModuleConstants.ModuleName} weawing aborted!");
        }
    }
}