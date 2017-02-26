using NUnit.Framework;

namespace NUnitTestTimeLimiter.Fody.Tests
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
