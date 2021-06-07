using System.IO;
using System.Runtime.InteropServices;

namespace OTAPI.Common
{
    public class MacOSInstallDiscoverer : BaseInstallDiscoverer
    {
        public override string[] SearchPaths { get; } = new[]
        {
            "/Users/[USER_NAME]/Library/Application Support/Steam/steamapps/common/Terraria/Terraria.app/Contents/",
            "/Applications/Terraria.app/Contents/",
        };

        public override OSPlatform GetClientPlatform() => OSPlatform.OSX;

        public override string GetResource(string fileName, string installPath) => Path.Combine(installPath, "Resources", fileName);
        public override string GetResourcePath(string installPath) => Path.Combine(installPath, "Resources");

        public override bool IsValidInstallPath(string folder)
        {
            bool valid = Directory.Exists(folder);

            var macOS = Path.Combine(folder, "MacOS");
            var resources = Path.Combine(folder, "Resources");

            var startScript = Path.Combine(macOS, "Terraria");
            var startBin = Path.Combine(macOS, "Terraria.bin.osx");
            var assembly = Path.Combine(resources, "Terraria.exe");

            valid &= Directory.Exists(macOS);
            valid &= Directory.Exists(resources);

            valid &= File.Exists(startScript);
            valid &= File.Exists(startBin);
            valid &= File.Exists(assembly);

            return valid;
        }
    }
}
