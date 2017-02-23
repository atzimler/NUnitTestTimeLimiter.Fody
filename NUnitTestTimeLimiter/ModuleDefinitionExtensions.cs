using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody
{
    public static class ModuleDefinitionExtensions
    {
        public static AssemblyDefinition AssemblyDefinition(this ModuleDefinition moduleDefinition)
        {
            if (moduleDefinition == null)
            {
                throw new ArgumentNullException(nameof(moduleDefinition));
            }

            return moduleDefinition.Assembly;
        }

        public static TypeDefinition ImportDefinition(this ModuleDefinition moduleDefinition, TypeReference typeRefence)
        {
            var assemblies = moduleDefinition?.Assembly?.ReferencedAssemblies() ??
                             Enumerable.Empty<AssemblyDefinition>();
            var modules = assemblies.SelectMany(a => a.ModuleDefinitions()).Where(m => m != null);
            var types = modules.SelectMany(m => m.Types);
            var typeDefinitions = types.Where(t => t?.FullName == typeRefence?.FullName).Take(2).ToList();
            return typeDefinitions.Count != 1 ? null : typeDefinitions[0];
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<TypeDefinition> TypeDefinitionsWithAttribute(
            [NotNull] [ItemNotNull] this IEnumerable<ModuleDefinition> moduleDefinitions,
            TypeReference typeReference)
        {
            return
                moduleDefinitions.SelectMany(md => md.Types)
                    .Where(t => t != null)
                    .Where(t => t.HasAttribute(typeReference));
        }
    }
}
