using JetBrains.Annotations;
using Mono.Cecil;
using System;
using System.Linq;

namespace NUnitTestTimeLimiter.Fody
{
    public static class TypeExtensions
    {
        public static TypeDefinition TypeDefinition([NotNull] this Type type, [NotNull] ModuleDefinition module)
        {
            var assemblies = module.Assembly?.ReferencedAssemblies() ?? Enumerable.Empty<AssemblyDefinition>();
            var modules = assemblies.SelectMany(a => a.ModuleDefinitions()).Where(m => m != null);
            var types = modules.SelectMany(m => m.Types);
            var typeDefinitions = types.Where(t => t?.FullName == type.FullName).ToList();
            return typeDefinitions.Count != 1 ? null : typeDefinitions[0];
        }

        public static TypeReference TypeReference([NotNull] this Type type, [NotNull] ModuleDefinition module)
        {
            return TypeDefinition(type, module);
        }
    }
}
