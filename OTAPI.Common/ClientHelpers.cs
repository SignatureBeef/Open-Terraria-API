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
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Common
{
    public class ClientInstallPath<ITarget>
        where ITarget : IInstallDiscoverer
    {
        public ITarget Target { get; set; }
        public string Path { get; set; }
        public string GetResource(string fileName) => Target.GetResource(fileName, Path);
        public string GetResourcePath() => Target.GetResourcePath(Path);
    }

    public class ClientHelpers
    {
        static IInstallDiscoverer[] Discoverers = new IInstallDiscoverer[]
        {
            new MacOSInstallDiscoverer(),
            new WindowsInstallDiscoverer(),
            new LinuxInstallDiscoverer(),
        };

        public static ClientInstallPath<IInstallDiscoverer> DetermineClientInstallPath() => DetermineClientInstallPath(Discoverers);

        public static ClientInstallPath<ITarget> DetermineClientInstallPath<ITarget>(ITarget[] discoverers)
            where ITarget : IInstallDiscoverer
        {
            var installPaths = discoverers
                .SelectMany(x => x.FindInstalls().Select(i => new ClientInstallPath<ITarget> { Path = i, Target = x }));

            if (installPaths.Count() == 1)
                return installPaths.Single();
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
                        return installPaths.ElementAt(index);
                    else Console.Error.WriteLine("Invalid option: " + index);
                }
                else Console.Error.WriteLine("Invalid option, expected a number.");
            }

            throw new DirectoryNotFoundException();
        }

        public static ClientInstallPath<IInstallDiscoverer>? FromPath(string directory)
        {
            foreach (var discover in Discoverers)
            {
                if (discover.IsValidInstallPath(directory))
                {
                    return new ClientInstallPath<IInstallDiscoverer> { Path = directory, Target = discover };
                }
            }

            throw new Exception($"Unable to determine the discoverer for path: {directory}");
        }
    }
}
