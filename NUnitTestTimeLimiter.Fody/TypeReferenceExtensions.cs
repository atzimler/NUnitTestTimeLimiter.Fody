using Mono.Cecil;

namespace NUnitTestTimeLimiter.Fody
{
    public static class TypeReferenceExtensions
    {
        public static bool EqualsFullName(TypeReference left, TypeReference right)
        {
            return left != null && right != null && left.FullName == right.FullName;
        }
    }
}
