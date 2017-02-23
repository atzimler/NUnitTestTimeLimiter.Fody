using System.Linq;
using Mono.Cecil;
using NUnit.Framework;
using NUnitTestTimeLimiter.Fody;

namespace Tests
{
    [TestFixture]
    public class TypeExtensionsShould : WeaverTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializeAssemblyReferences();
        }

        [Test]
        public void GiveBackCorrectTypeReference()
        {
            Assert.IsNotNull(ModuleDefinition);
            Assert.IsNotNull(NUnitModuleDefinition?.Assembly);

            var testFixtureAttribute = typeof(TestFixtureAttribute).TypeReference(ModuleDefinition);
            var assembly = NUnitModuleDefinition.Assembly;
            var modules = assembly.Modules;
            var types = modules?.SelectMany(m => m?.Types) ?? Enumerable.Empty<TypeReference>();

            var correct = types.Any(t => t?.FullName == testFixtureAttribute?.FullName);
            Assert.IsTrue(correct);
        }

    }
}
