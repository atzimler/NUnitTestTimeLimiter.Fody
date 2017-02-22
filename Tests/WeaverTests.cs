using NUnit.Framework;
using System.Reflection;

namespace Tests
{
    [TestFixture]
    public class WeaverTests : WeaverTestBase
    {
        private Assembly _assembly;

        [OneTimeSetUp]
        public void Setup()
        {
            InitializeAssemblyReferences();

            if (ModuleDefinition == null)
            {
                Assert.Fail($"Could not find the weaver output assembly: {NewAssemblyPath}");
            }
            var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = ModuleDefinition
            };

            weavingTask.Execute();
            ModuleDefinition.Write(NewAssemblyPath);

            _assembly = Assembly.LoadFile(NewAssemblyPath);
        }

        [Test]
        public void ValidateHelloWorldIsInjected()
        {
            //var type = _assembly.GetType("Hello");
            //var instance = (dynamic)Activator.CreateInstance(type);

            //Assert.AreEqual("Hello World", instance.World());
        }

#if(DEBUG)
        [Test]
        public void PeVerify()
        {
            Assert.IsNotNull(AssemblyPath);
            Assert.IsNotNull(NewAssemblyPath);
            Verifier.Verify(AssemblyPath, NewAssemblyPath);
        }
#endif
    }
}