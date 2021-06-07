using OTAPI.Common;

namespace OTAPI.Client.Installer.Targets
{
    public interface IInstallTarget : IInstallDiscoverer
    {
        void Install(string installPath);
    }
}
