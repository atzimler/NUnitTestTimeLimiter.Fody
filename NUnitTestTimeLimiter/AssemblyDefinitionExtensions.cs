using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody
{
    public static class AssemblyDefinitionExtensions
    {
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<ModuleDefinition> GetModuleDefinitions(this AssemblyDefinition assemblyDefinition)
        {
            var modules = assemblyDefinition?.Modules ?? Enumerable.Empty<ModuleDefinition>();
            return modules.Where(m => m != null);
        }

    }
}

