using OTAPI.Common;
using System;

namespace OTAPI.Client.Installer.Targets
{
    public interface IInstallTarget : IInstallDiscoverer
    {
        void Install(string installPath);
        bool IsValidInstallPath(string installPath);
        event EventHandler<InstallStatusUpdate> StatusUpdate;
        string Status { set; }
    }
}
