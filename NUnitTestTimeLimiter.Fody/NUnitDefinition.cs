using System;
using JetBrains.Annotations;
using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody
{
    public class NUnitDefinition
    {
        private const string NUnitFrameworkAssembly = "nunit.framework";
        private const string NUnitFrameworkNamespace = "NUnit.Framework";
        private const string TimeoutAttributeName = "TimeoutAttribute";
        private const string TestFixtureAttributeName = "TestFixtureAttribute";

        public TypeDefinition TimeoutAttribute { get; }
        public TypeDefinition TestFixtureAttribute { get; }

        public string TimeoutAttributeFullName => TimeoutAttribute?.FullName;

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

        public NUnitDefinition(ModuleDefinition moduleDefinition)
        {
            var nunitAssembly = moduleDefinition?.ReferencedAssembly(NUnitFrameworkAssembly);
            if (nunitAssembly == null)
            {
                return;
            }

            TimeoutAttribute = LoadTypeDefinition(nunitAssembly, NUnitFrameworkNamespace, TimeoutAttributeName);
            TestFixtureAttribute = LoadTypeDefinition(nunitAssembly, NUnitFrameworkNamespace, TestFixtureAttributeName);
        }


    }
}
