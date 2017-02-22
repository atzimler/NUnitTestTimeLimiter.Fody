using JetBrains.Annotations;
using Mono.Cecil;
using System;
using System.Linq;

namespace NUnitTestTimeLimiter.Fody
{
    public static class TypeExtensions
    {
        public static AssemblyNameReference AssemblyNameReference([NotNull] this Type type)
        {
            var assembly = type.Assembly;
            var fullName = assembly.FullName;
            var name = assembly.GetName();
            var version = name.Version;

            return new AssemblyNameReference(fullName, version);
        }

        public static TypeDefinition TypeDefinition([NotNull] this Type type, ModuleDefinition module)
        {
            var assembly = module.GetAssemblyDefinition();
            var modules = assembly.GetModuleDefinitions();
            var types = modules.SelectMany(t => t.Types);
            var typeDefinitions = types.Where(t => t?.FullName == type.FullName).ToList();
            return typeDefinitions.Count != 1 ? null : typeDefinitions[0];
        }

        public static TypeReference TypeReference([NotNull] this Type type, ModuleDefinition module)
        {
            return TypeDefinition(type, module);
        }
    }
}
