using AssemblyToProcess;
using Mono.Cecil;
using NUnit.Framework;
using NUnitTestTimeLimiter.Fody;

namespace Tests
{
    [TestFixture]
    public class TypeDefinitionExtensionsShould : WeaverTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializeAssemblyReferences();
        }

        [Test]
        public void ProperlyDetectIfTypeHasAnAttribute()
        {
            var assemblyToProcessModuleDefinition = ModuleDefinition.ReadModule(AssemblyPath);
            var testFixtureClass = typeof(TestFixtureWithoutTimeout).TypeDefinition(assemblyToProcessModuleDefinition);
            var testFixtureAttribute = ModuleDefinition.ImportReference(typeof(TestFixtureAttribute));

            Assert.IsTrue(testFixtureClass?.HasAttribute(testFixtureAttribute));
        }

        [Test]
        public void ProperlyDetectIfTypeDoesNotHaveAnAttribute()
        {
            var assemblyToProcessModuleDefinition = ModuleDefinition.ReadModule(AssemblyPath);
            var testFixtureClass = typeof(TestFixtureWithoutTimeout).TypeDefinition(assemblyToProcessModuleDefinition);
            var timeoutAttribute = ModuleDefinition.ImportReference(typeof(TimeoutAttribute));

            Assert.IsFalse(testFixtureClass?.HasAttribute(timeoutAttribute));
        }
    }
}
