using System;
using System.IO;
using JetBrains.Annotations;
using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody.Tests
{
    public class WeaverTestBase
    {
        protected ModuleDefinition ModuleDefinition;
        protected string NewAssemblyPath { get; private set; }
        protected string AssemblyPath { get; private set; }

        protected void InitializeAssemblyReferences([NotNull] string assemblyName = @"AssemblyToProcess.dll")
        {
            AssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);
#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

            NewAssemblyPath = AssemblyPath.Replace(".dll", "2.dll");
            File.Copy(AssemblyPath, NewAssemblyPath, true);

            ModuleDefinition = ModuleDefinition.ReadModule(NewAssemblyPath);
        }
    }
}