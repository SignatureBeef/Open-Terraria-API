using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OTAPI.Common
{
    public abstract class BaseInstallDiscoverer : IInstallDiscoverer
    {
        public abstract string[] SearchPaths { get; }

        public abstract bool IsValidInstallPath(string folder);

        public abstract OSPlatform GetClientPlatform();

        public abstract string GetResource(string fileName, string installPath);
        public abstract string GetResourcePath(string installPath);

        public virtual IEnumerable<string> FindInstalls()
        {
            foreach (var path in SearchPaths)
            {
                var formatted = path.Replace("[USER_NAME]", Environment.UserName);
                if (IsValidInstallPath(formatted))
                    yield return formatted;
            }
        }
    }
}
