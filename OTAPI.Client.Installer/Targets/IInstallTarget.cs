using System;
using System.Collections.Generic;

namespace OTAPI.Client.Installer.Targets
{
    public interface IInstallTarget
    {
        IEnumerable<string> FindInstalls();

        void Install(string installPath);
    }
}
