﻿using JetBrains.Annotations;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NUnitTestTimeLimiter.Fody
{
    public static class AssemblyDefinitionExtensions
    {
        private static AssemblyDefinition ResolveAssemblyNameReference([NotNull] string assemblyFullName)
        {
            var assembly = Assembly.Load(assemblyFullName);
            var assemblyUri = new Uri(assembly.CodeBase);
            if (!assemblyUri.IsFile)
            {
                return null;
            }

            var assemblyFilePath = assemblyUri.LocalPath;
            var moduleDefinition = ModuleDefinition.ReadModule(assemblyFilePath);
            return moduleDefinition.Assembly;
        }

        private static void MapAssemblyReferences(string assemblyFullName, [NotNull] Queue<string> unprocessedAssemblies)
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

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<ModuleDefinition> ModuleDefinitions(this AssemblyDefinition assemblyDefinition)
        {
            var modules = assemblyDefinition?.Modules ?? Enumerable.Empty<ModuleDefinition>();
            return modules.Where(m => m != null);
        }

        public static IEnumerable<AssemblyDefinition> ReferencedAssemblies([NotNull] this AssemblyDefinition assemblyDefinition)
        {
            var unprocessedAssemblies = new Queue<string>();
            var processedAssemblies = new List<string>();

            unprocessedAssemblies.Enqueue(assemblyDefinition.FullName);
            while (unprocessedAssemblies.Count > 0)
            {
                var assemblyFullName = unprocessedAssemblies.Dequeue();
                if (processedAssemblies.Contains(assemblyFullName))
                {
                    continue;
                }

                MapAssemblyReferences(assemblyFullName, unprocessedAssemblies);

                processedAssemblies.Add(assemblyFullName);
                yield return ResolveAssemblyNameReference(assemblyFullName);
            }
        }

    }
}

