using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody
{
    public static class ModuleDefinitionExtensions
    {
        public static AssemblyDefinition GetAssemblyDefinition(this ModuleDefinition moduleDefinition)
        {
            if (moduleDefinition == null)
            {
                throw new ArgumentNullException(nameof(moduleDefinition));
            }

            return moduleDefinition.Assembly;
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<TypeDefinition> GetTypeDefinitionsWithAttribute(
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
