using System;
using System.IO;
using Mono.Cecil;

namespace Tests
{
    public class WeaverTestBase
    {
        protected ModuleDefinition ModuleDefinition;
        protected string NewAssemblyPath { get; private set; }
        protected string AssemblyPath { get; private set; }

        protected void InitializeAssemblyReferences()
        {
            var testDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var projectPath = Path.GetFullPath(
                Path.Combine(testDirectory, @"..\..\..\AssemblyToProcess\AssemblyToProcess.csproj"));
            AssemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

            NewAssemblyPath = AssemblyPath.Replace(".dll", "2.dll");
            File.Copy(AssemblyPath, NewAssemblyPath, true);

            ModuleDefinition = ModuleDefinition.ReadModule(NewAssemblyPath);
        }
    }
}