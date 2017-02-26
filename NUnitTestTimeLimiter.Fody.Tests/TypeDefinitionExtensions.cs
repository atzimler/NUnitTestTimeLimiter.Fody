using AssemblyToProcess;
using Mono.Cecil;
using NUnit.Framework;

namespace NUnitTestTimeLimiter.Fody.Tests
{
    [TestFixture]
    public class TypeDefinitionExtensions : WeaverTestBase
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
            var testFixtureClassReference = assemblyToProcessModuleDefinition?.ImportReference(typeof(TestFixtureWithoutTimeout));
            var testFixtureClassDefinition = ModuleDefinition.ImportDefinition(testFixtureClassReference);
            var testFixtureAttribute = ModuleDefinition?.ImportReference(typeof(TestFixtureAttribute));

            Assert.IsTrue(testFixtureClassDefinition?.HasAttribute(testFixtureAttribute));
        }

        [Test]
        public void ProperlyDetectIfTypeDoesNotHaveAnAttribute()
        {
            var assemblyToProcessModuleDefinition = ModuleDefinition.ReadModule(AssemblyPath);
            var testFixtureClassReference = assemblyToProcessModuleDefinition?.ImportReference(typeof(TestFixtureWithoutTimeout));
            var testFixtureClassDefinition = ModuleDefinition.ImportDefinition(testFixtureClassReference);
            var timeoutAttribute = ModuleDefinition?.ImportReference(typeof(TimeoutAttribute));

            Assert.IsFalse(testFixtureClassDefinition?.HasAttribute(timeoutAttribute));
        }
    }
}
