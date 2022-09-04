/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace OTAPI.Common;

public class WindowsInstallDiscoverer : BaseInstallDiscoverer
{
    public override string[] SearchPaths { get; } = new[]
    {
        "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Terraria"
    };

    public override OSPlatform GetClientPlatform() => OSPlatform.Windows;

    public override string GetResource(string fileName, string installPath) => Path.Combine(installPath, fileName);
    public override string GetResourcePath(string installPath) => installPath;

    public override bool IsValidInstallPath(string folder)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return false;

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

    public override bool VerifyIntegrity(string path)
    {
        var hash = IntegrityHashes.ComputeHash(path);
        return IntegrityHashes.WindowsClient.Any(h => h.Equals(hash, StringComparison.CurrentCultureIgnoreCase));
    }
}
