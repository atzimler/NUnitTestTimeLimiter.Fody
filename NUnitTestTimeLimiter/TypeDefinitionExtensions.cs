using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody
{
    public static class TypeDefinitionExtensions
    {
        public static bool HasAttribute([NotNull] this TypeDefinition typeDefinition, TypeReference attributeType)
        {
            var customAttributes = typeDefinition.CustomAttributes;
            return customAttributes != null && customAttributes.Any(ca => ca?.AttributeType?.FullName == attributeType?.FullName);
        }
    }
}