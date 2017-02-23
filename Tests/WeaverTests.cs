using System;
using System.Collections.Generic;
using AssemblyToProcess;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using JetBrains.Annotations;

// FIXME: TypeSystem.LookupType(): TypeReference is probably a better implementation of mine.
namespace Tests
{
    [TestFixture]
    public class WeaverTests : WeaverTestBase
    {
        private Assembly _assembly;

        [NotNull]
        private IEnumerable<TimeoutAttribute> GetTimeoutAttributes([NotNull] Type type)
        {
            return _assembly?
                       .GetType(type.FullName)?
                       .GetCustomAttributes(typeof(TimeoutAttribute), true)
                       .Select(o => o as TimeoutAttribute) ?? Enumerable.Empty<TimeoutAttribute>();
        }

        private bool HasTimeoutAttribute([NotNull] Type type)
        {
            return GetTimeoutAttributes(type).Any();
        }

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
        public void NonTestFixtureClassIsNotAttributedWithTimeout()
        {
            Assert.IsFalse(HasTimeoutAttribute(typeof(NonTestFixtureClass)));
        }

        [Test]
        public void TestFixtureWithoutTimeoutIsModifiedToHaveTimeout()
        {
            Assert.IsTrue(HasTimeoutAttribute(typeof(TestFixtureWithoutTimeout)));
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