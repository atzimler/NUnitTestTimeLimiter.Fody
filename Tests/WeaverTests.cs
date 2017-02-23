using System;
using System.Collections.Generic;
using AssemblyToProcess;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

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

        private int? Timeout([NotNull] Type type)
        {
            var attribute = GetTimeoutAttributes(type).FirstOrDefault();
            if (attribute == null)
            {
                return null;
            }

            var fieldInfo = typeof(TimeoutAttribute).GetField("_timeout", BindingFlags.NonPublic | BindingFlags.Instance);
            var value = fieldInfo?.GetValue(attribute);
            return (int?)value;
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
        public void TestFixtureWithLowerTimeoutIsNotModified()
        {
            Assert.IsTrue(HasTimeoutAttribute(typeof(TestFixtureWithLowerTimeout)));
            Assert.AreEqual(1000, Timeout(typeof(TestFixtureWithLowerTimeout)));
        }

        [Test]
        public void TestFixtureWithCorrectTimeoutIsUnchanged()
        {
            Assert.IsTrue(HasTimeoutAttribute(typeof(TestFixtureWithCorrectTimeout)));
            Assert.AreEqual(2000, Timeout(typeof(TestFixtureWithCorrectTimeout)));
        }

        [Test]
        public void TestFixtureWithHigherTimeoutIsAdjusted()
        {
            Assert.IsTrue(HasTimeoutAttribute(typeof(TestFixtureWithHigherTimeout)));
            Assert.AreEqual(2000, Timeout(typeof(TestFixtureWithHigherTimeout)));
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