using System;

namespace NUnitTestTimeLimiter.Fody
{
    public class AssemblyUri
    {
        private readonly Uri _uri;

        public bool IsFile => _uri?.IsFile ?? false;
        public string LocalPath => _uri?.LocalPath;

        public AssemblyUri(string uriString)
        {
            if (string.IsNullOrEmpty(uriString))
            {
                return;
            }

            _uri = new Uri(uriString);
        }
    }
}
