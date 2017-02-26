using System;
using System.IO;
using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody.Tests
{
    public class WeaverTestBase
    {
        protected ModuleDefinition ModuleDefinition;
        protected string NewAssemblyPath { get; private set; }
        protected string AssemblyPath { get; private set; }

        protected void InitializeAssemblyReferences()
        {
            AssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AssemblyToProcess.dll");
#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

            NewAssemblyPath = AssemblyPath.Replace(".dll", "2.dll");
            File.Copy(AssemblyPath, NewAssemblyPath, true);

            ModuleDefinition = ModuleDefinition.ReadModule(NewAssemblyPath);
        }
    }
}