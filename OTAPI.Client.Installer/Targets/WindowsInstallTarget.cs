using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace OTAPI.Client.Installer.Targets
{
    public class WindowsInstallTarget : BaseInstallTarget
    {
        public override string[] SearchPaths { get; } = new[]
        {
            "C:\\Program Files (x86)\\Steam\\steamapps\\common"
        };

        public override bool IsValidInstallPath(string folder)
        {
            bool valid = Directory.Exists(folder);

            var macOS = Path.Combine(folder, "MacOS");
            var resources = Path.Combine(folder, "Resources");

            var startScript = Path.Combine(macOS, "Terraria");
            var startBin = Path.Combine(macOS, "Terraria.bin.osx");
            //var assembly = Path.Combine(resources, "Terraria.exe");

            valid &= Directory.Exists(macOS);
            valid &= Directory.Exists(resources);

            valid &= File.Exists(startScript);
            valid &= File.Exists(startBin);
            //valid &= File.Exists(assembly);

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

        public override void Install(string installPath)
        {

        }
    }
}
