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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace OTAPI.Client.Launcher.Actions
{
    static class Vanilla
    {
        public static void Launch(string installPath, string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var script = Path.Combine(installPath, "MacOS", "Terraria");
                System.Console.WriteLine($"Launching: {script}");
                using var process = new Process();
                process.StartInfo.FileName = script;
                process.Start();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var script = Path.Combine(installPath, "Terraria");
                System.Console.WriteLine($"Launching: {script}");
                using var process = new Process();
                process.StartInfo.FileName = script;
                process.Start();
            }
            else
            {
                var script = Path.Combine(installPath, "Terraria.exe");
                System.Console.WriteLine($"Launching: {script}");
                Process.Start(new ProcessStartInfo()
                {
                    WorkingDirectory = installPath,
                    FileName = script
                });
            }
        }
    }
}
