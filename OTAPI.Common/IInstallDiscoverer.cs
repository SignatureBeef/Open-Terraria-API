using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OTAPI.Common
{
    public interface IInstallDiscoverer
    {
        IEnumerable<string> FindInstalls();

        OSPlatform GetClientPlatform();

        string GetResource(string fileName, string installPath);
        string GetResourcePath(string installPath);
    }
}
