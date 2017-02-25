using NUnit.Framework;
using NUnitTestTimeLimiter.Fody;

namespace Tests
{
    [TestFixture]
    public class NUnitDefinitionShould : WeaverTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializeAssemblyReferences();
        }

        [Test]
        public void TimeoutAttributeIsResolved()
        {
            var nunitDefinition = new NUnitDefinition(ModuleDefinition);
            Assert.IsNotNull(nunitDefinition.TimeoutAttribute);
        }

    }
}
