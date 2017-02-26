using NUnit.Framework;
using NUnitTestTimeLimiter.Fody;
using System;
using System.Linq;

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

        [Test]
        public void ResolveAssemblyCorrectlyIfExists()
        {
            var assembly = ModuleDefinition.ReferencedAssembly("nunit.framework");
            Assert.IsNotNull(assembly);
        }

        [Test]
        public void ResolveAssemblyCorrectlyIfExistsButWithDifferentCase()
        {
            Assert.IsNotNull(ModuleDefinition.ReferencedAssembly("NUniT.FrAmEwOrK"));
        }

        [Test]
        public void UnresolveNonExistingReference()
        {
            Assert.IsNull(ModuleDefinition.ReferencedAssembly("This.Should.Not.Be.A.Valid.Assembly"));
        }

    }
}
