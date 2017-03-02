using NUnit.Framework;
using System.Linq;

namespace NUnitTestTimeLimiter.Fody.Tests
{
    [TestFixture]
    public class AccidentalDllSkipAnalysis : WeaverTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitializeAssemblyReferences(@"TestComponents\Tryout2.dll");
        }

        [Test]
        public void WhyTryout2DllsNUnitReferenceIsNotDetected()
        {
            Assert.IsNotNull(ModuleDefinition);

            //FIXME: Assembly is not found by the resolver by its full name, and as a result it returns null.
            //Assembly load failure should be handled as described in: https://msdn.microsoft.com/en-us/library/ff527268.aspx

            var referencedAssemblies = ModuleDefinition.Assembly?.ReferencedAssemblies().ToList();
            Assert.IsNotNull(referencedAssemblies);
            //Assert.AreEqual(1, referencedAssemblies.Count, "Referenced assembly count is incorrect!");

            Assert.IsNotNull(ModuleDefinition.ReferencedAssembly("nunit.framework"),
                "Reference to nunit.framework was not found by ModuleDefinition.ReferencedAssembly!");

            var nunitDefinition = new NUnitDefinition(ModuleDefinition);
            Assert.IsTrue(nunitDefinition.NUnitPresent, $"NUnitDefinition.NUnitPresent is expected to be true!");
        }
    }
}
