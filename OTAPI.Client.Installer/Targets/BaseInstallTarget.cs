using System;
using System.Collections.Generic;
using System.IO;

namespace OTAPI.Client.Installer.Targets
{
    public abstract class BaseInstallTarget : IInstallTarget
    {
        public abstract string[] SearchPaths { get; }

        public abstract bool IsValidInstallPath(string folder);

        public virtual IEnumerable<string> FindInstalls()
        {
            foreach (var path in SearchPaths)
            {
                var formatted = path.Replace("[USER_NAME]", Environment.UserName);
                if (IsValidInstallPath(formatted))
                    yield return formatted;
            }
        }

        public abstract void Install(string installPath);
    }
}
