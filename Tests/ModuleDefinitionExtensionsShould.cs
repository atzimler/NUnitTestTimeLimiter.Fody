using System;
using System.Linq;
using NUnit.Framework;
using NUnitTestTimeLimiter.Fody;

namespace Tests
{
    [TestFixture]
    public class ModuleDefinitionExtensionsShould : WeaverTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializeAssemblyReferences();
        }

        [Test]
        public void ThrowArgumentNullExceptionIfModuleDefinitionIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ModuleDefinitionExtensions.AssemblyDefinition(null));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: moduleDefinition", ex?.Message);
        }

        [Test]
        public void EnumerateTheCorrectNumberOfClassesWithTestFixtureAttribute()
        {
            var testFixtureAttribute = ModuleDefinition?.ImportReference(typeof(TestFixtureAttribute));
            var assemblyDefinition = ModuleDefinition.AssemblyDefinition();
            var moduleDefinitions = assemblyDefinition.ModuleDefinitions();
            var types = moduleDefinitions.TypeDefinitionsWithAttribute(testFixtureAttribute);
            Assert.AreEqual(4, types.Count());
        }
    }
}
