using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace OTAPI.Common
{
    public class WindowsInstallDiscoverer : BaseInstallDiscoverer
    {
        public override string[] SearchPaths { get; } = new[]
        {
            "C:\\Program Files (x86)\\Steam\\steamapps\\common"
        };

        public override OSPlatform GetClientPlatform() => OSPlatform.Windows;

        public override string GetResource(string fileName, string installPath) => Path.Combine(installPath, fileName);
        public override string GetResourcePath(string installPath) => installPath;

        public override bool IsValidInstallPath(string folder)
        {
            bool valid = Directory.Exists(folder);

            var content = Path.Combine(folder, "Content");

            var clientExe = Path.Combine(folder, "Terraria.exe");
            var serverExe = Path.Combine(folder, "TerrariaServer.exe");

            valid &= Directory.Exists(content);

            valid &= File.Exists(clientExe);
            valid &= File.Exists(serverExe);

            return valid;
        }

        [SupportedOSPlatform("windows")]
        IEnumerable<string> DiscoverViaSteam()
        {
            // use registry to resolve a user-defined path.
            var steamInstall = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string;

            if (!String.IsNullOrWhiteSpace(steamInstall) && Directory.Exists(steamInstall))
            {
                var VDF = Path.Combine(steamInstall, "steamapps", "libraryfolders.vdf");

                if (File.Exists(VDF))
                {
                    var lines = File.ReadAllLines(VDF);
                    var libraryFolders = lines
                        .Where(x => x.IndexOf(":\\\\") > -1)
                        .Select(line => line.Split('"').Single(x => x.IndexOf(":\\\\") > -1).Replace("\\\\", "\\"))
                        .ToArray();

                    foreach (var folder in libraryFolders)
                    {
                        var Terraria = Path.Combine(folder, "steamapps", "common", "Terraria");
                        if (Directory.Exists(Terraria))
                        {
                            if (IsValidInstallPath(Terraria))
                                yield return Terraria;
                        }
                    }
                }
            }
        }

        public override IEnumerable<string> FindInstalls()
        {
            var existing = base.FindInstalls();
            if (existing.Any())
                return existing;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return DiscoverViaSteam();
            }

            return Enumerable.Empty<string>();
        }
    }
}
