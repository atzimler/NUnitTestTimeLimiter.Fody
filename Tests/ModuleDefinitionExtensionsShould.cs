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
            var ex = Assert.Throws<ArgumentNullException>(() => ModuleDefinitionExtensions.GetAssemblyDefinition(null));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: moduleDefinition", ex?.Message);
        }

        [Test]
        public void EnumerateTheCorrectNumberOfClassesWithTestFixtureAttribute()
        {
            var testFixtureAttribute = typeof(TestFixtureAttribute).TypeReference(NUnitModuleDefinition);
            var assemblyDefinition = ModuleDefinition.GetAssemblyDefinition();
            var moduleDefinitions = assemblyDefinition.GetModuleDefinitions();
            var types = moduleDefinitions.GetTypeDefinitionsWithAttribute(testFixtureAttribute);
            Assert.AreEqual(4, types.Count());
        }
    }
}
