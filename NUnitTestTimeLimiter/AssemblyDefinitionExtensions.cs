using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody
{
    public static class AssemblyDefinitionExtensions
    {
        private static void MapAssemblyReferences(AssemblyDefinition assembly, [NotNull] Queue<AssemblyDefinition> unprocessedAssemblies)
        {
            var mainModule = assembly?.MainModule;
            var references = mainModule?.AssemblyReferences ?? Enumerable.Empty<AssemblyNameReference>();
            references.ToList().ForEach(r =>
            {
                var referencedAssembly = mainModule?.AssemblyResolver?.Resolve(r);
                if (referencedAssembly != null)
                {
                    unprocessedAssemblies.Enqueue(referencedAssembly);
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
            var unprocessedAssemblies = new Queue<AssemblyDefinition>();
            var processedAssemblies = new List<AssemblyDefinition>();

            unprocessedAssemblies.Enqueue(assemblyDefinition);
            while (unprocessedAssemblies.Count > 0)
            {
                var assembly = unprocessedAssemblies.Dequeue();
                if (processedAssemblies.Contains(assembly))
                {
                    continue;
                }

                MapAssemblyReferences(assembly, unprocessedAssemblies);

                processedAssemblies.Add(assembly);
                yield return assembly;
            }
        }

    }
}

