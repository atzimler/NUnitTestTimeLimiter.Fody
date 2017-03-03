﻿using JetBrains.Annotations;
using Mono.Cecil;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NUnitTestTimeLimiter.Fody
{
    public static class AssemblyDefinitionExtensions
    {
        private static void MapAssemblyReferences([NotNull] string assemblyFullName, [NotNull] Queue<string> unprocessedAssemblies)
        {
            var assembly = ResolveAssemblyNameReference(assemblyFullName);
            var mainModule = assembly?.MainModule;
            var references = mainModule?.AssemblyReferences ?? Enumerable.Empty<AssemblyNameReference>();

            references.Where(r => r != null).Select(r => r.FullName).ToList().ForEach(r =>
            {
                if (!unprocessedAssemblies.Contains(r))
                {
                    unprocessedAssemblies.Enqueue(r);
                }
            });
        }

        private static AssemblyDefinition ResolveAssemblyNameReference([NotNull] string assemblyFullName)
        {
            var assemblyName = new AssemblyName(assemblyFullName);
            var assemblyFilePath = Directory.GetFiles(
                    Directory.GetCurrentDirectory(),
                    $"{assemblyName.Name}.dll",
                    SearchOption.AllDirectories
                )
                .FirstOrDefault();
            if (assemblyFilePath == null)
            {
                return null;
            }

            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFilePath);
            return moduleDefinition?.Assembly;
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<ModuleDefinition> ModuleDefinitions(this AssemblyDefinition assemblyDefinition)
        {
            var modules = assemblyDefinition?.Modules ?? Enumerable.Empty<ModuleDefinition>();
            return modules.Where(m => m != null);
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<AssemblyDefinition> ReferencedAssemblies([NotNull] this AssemblyDefinition assemblyDefinition)
        {
            var unprocessedAssemblies = new Queue<string>();
            var processedAssemblies = new List<string>();

            unprocessedAssemblies.Enqueue(assemblyDefinition.FullName);
            while (unprocessedAssemblies.Count > 0)
            {
                var assemblyFullName = unprocessedAssemblies.Dequeue();
                if (assemblyFullName == null || processedAssemblies.Contains(assemblyFullName))
                {
                    continue;
                }

                MapAssemblyReferences(assemblyFullName, unprocessedAssemblies);

                processedAssemblies.Add(assemblyFullName);
                var item = ResolveAssemblyNameReference(assemblyFullName);
                if (item != null)
                {
                    yield return item;
                }
            }
        }

    }
}

