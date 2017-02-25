using System;
using Mono.Cecil;
using NUnit.Framework;
using NUnitTestTimeLimiter.Fody;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class AssemblyDefinitionExtensionsShould : WeaverTestBase
    {
        private AssemblyDefinition _assemblyDefinition;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializeAssemblyReferences();
            _assemblyDefinition = ModuleDefinition?.Assembly;
        }

        [Test]
        public void ReturnEmptySequenceOfModuleDefinitionsIfAssemblyDefinitionIsNull()
        {
            Assert.AreEqual(0, AssemblyDefinitionExtensions.ModuleDefinitions(null).Count());
        }

        [Test]
        public void ReturnCorrectModules()
        {
            var modules = _assemblyDefinition.ModuleDefinitions().ToList();
            Assert.AreEqual(1, modules.Count);
            Assert.IsNotNull(modules[0]);
            Assert.AreEqual("AssemblyToProcess2.dll", Path.GetFileName(modules[0].FullyQualifiedName));
        }

        [Test]
        public void ReturnCorrectReferencedAssemblies()
        {
            var referencedAssemblies = _assemblyDefinition.ReferencedAssemblies();
            foreach (var assembly in referencedAssemblies)
            {
                Console.WriteLine(assembly.FullName);
            }
        }
    }
}
