using System;
using JetBrains.Annotations;

namespace NUnitTestTimeLimiter.Fody
{
    internal static class ModuleConstants
    {
        public const string ModuleName = "NUnitTestTimeLimiter";

        [NotNull]
        public static Action<string> LogWarning { get; set; } = s => { };
    }
}
