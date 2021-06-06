// Copyright (C) 2020-2021 DeathCradle
//
// This file is part of Open Terraria API v3 (OTAPI)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;

namespace OTAPI.Client.Installer
{
    class Program
    {
        static Targets.IInstallTarget[] Targets = new Targets.IInstallTarget[]
        {
            new Targets.MacOSInstallTarget(),
            new Targets.WindowsInstallTarget()
        };

        class InstallPath
        {
            public Targets.IInstallTarget Target { get; set; }
            public string Path { get; set; }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("NOTE: THIS IS NOT A REAL LAUNCHER. You must load Terraria yourself.");
            Console.WriteLine("This program will install the required files to your client directory.");

            var installPaths = Targets
                .SelectMany(x => x.FindInstalls().Select(i => new InstallPath { Path = i, Target = x }));

            InstallPath installationPath = null;
            if (installPaths.Count() == 1)
                installationPath = installPaths.Single();
            else if (installPaths.Count() > 1)
            {
                Console.WriteLine("More than one install path found; please specify which: ");

                for (var i = 0; i < installPaths.Count(); i++)
                {
                    Console.WriteLine($"\t{i} - {installPaths.ElementAt(i).Path}");
                }

                Console.Write("Choice: ");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (Int32.TryParse(key.KeyChar.ToString(), out int index))
                {
                    if (index < installPaths.Count() && index >= 0)
                    {
                        installationPath = installPaths.ElementAt(index);
                    }
                    else
                    {
                        Console.Error.WriteLine("Invalid option: " + index);
                        return;
                    }
                }
                else
                {
                    Console.Error.WriteLine("Invalid option, expected a number.");
                    return;
                }
            }

            if (installationPath != null)
            {
                Console.WriteLine($"Installing to {installationPath.Path}");

                installationPath.Target.Install(installationPath.Path);
            }
            else
            {
                Console.Error.WriteLine("No install path found");
            }

            //Environment.CurrentDirectory = otapiInstallPath;
            //var asm = System.Reflection.Assembly.LoadFile(Path.Combine(otapiInstallPath, "OTAPI.Client.Host.exe"));
            //asm.EntryPoint.Invoke(null, new object[] { new string[] { } });
        }
    }
}
