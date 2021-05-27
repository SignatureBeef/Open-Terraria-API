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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NuGet;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using OTAPI.Common;

namespace OTAPI.Client.Launcher
{
    class Program
    {
        public static void Main(string[] args)
        {
            var installPath = ClientHelpers.DetermineClientInstallPath();
            var hostFile = "../../../../OTAPI.Client.Host/bin/Debug/net472/OTAPI.Client.Host.exe";

            var output_host = Path.Combine(installPath, "Resources", "OTAPI.Client.Host.exe");
            var output_runtimes = Path.Combine(installPath, "Resources", "runtimes");
            var output_triton = Path.Combine(installPath, "Resources", "Triton.dll");

            if (!File.Exists(hostFile))
                throw new FileNotFoundException("Host binary not found, was it built before running the launcher?");

            InstallTritonRuntimes().Wait();

            var runtimes = "triton/runtimes";
            if (Directory.Exists(output_runtimes)) Directory.Delete(output_runtimes, true);
            Directory.Move(runtimes, output_runtimes);

            if (File.Exists(output_host)) File.Delete(output_host);
            File.Copy(hostFile, output_host);

            if (File.Exists(output_triton)) File.Delete(output_triton);
            File.Copy("triton/lib/net40/Triton.dll", output_triton);

            PatchOSXLaunch(installPath);
        }

        static async Task InstallTritonRuntimes()
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            string packageId = "Triton";
            NuGetVersion packageVersion = new NuGetVersion("1.0.0");
            using MemoryStream packageStream = new MemoryStream();

            await resource.CopyNupkgToStreamAsync(
                packageId,
                packageVersion,
                packageStream,
                cache,
                logger,
                cancellationToken);

            Console.WriteLine($"Downloaded package {packageId} {packageVersion}");

            using PackageArchiveReader packageReader = new PackageArchiveReader(packageStream);
            NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

            Console.WriteLine($"Tags: {nuspecReader.GetTags()}");
            Console.WriteLine($"Description: {nuspecReader.GetDescription()}");

            foreach (var file in packageReader.GetFiles())
            {
                packageReader.ExtractFile(file, Path.Combine("triton", file), logger);
            }
        }

        static void PatchOSXLaunch(string installPath)
        {
            {
                var launch_script = Path.Combine(installPath, "MacOS/Terraria");
                var backup_launch_script = Path.Combine(installPath, "MacOS/Terraria.bak.otapi");

                if (!File.Exists(backup_launch_script))
                {
                    File.Copy(launch_script, backup_launch_script);
                }

                var contents = File.ReadAllText(launch_script);
                var patched = contents.Replace("./Terraria.bin.osx $@", "./OTAPI.Client.Host.bin.osx $@");

                if (contents != patched)
                {
                    File.WriteAllText(launch_script, patched);
                }
            }

            {
                var bin = Path.Combine(installPath, "MacOS/Terraria.bin.osx");
                var patched_bin = Path.Combine(installPath, "MacOS/OTAPI.Client.Host.bin.osx");

                if (!File.Exists(patched_bin))
                {
                    File.Copy(bin, patched_bin);
                }
            }

            //{
            //    var src = Path.Combine(installPath, "Resources/Terraria.exe");
            //    var dst = Path.Combine(installPath, $"Resources/Terraria.orig.{DateTime.Now.Ticks}.exe");

            //    if (File.Exists(src))
            //        File.Move(src, dst);
            //}

            //{
            //    var src = Path.Combine(installPath, "Resources/TerrariaServer.exe");
            //    var dst = Path.Combine(installPath, $"Resources/TerrariaServer.orig.{DateTime.Now.Ticks}.exe");

            //    if (File.Exists(src))
            //        File.Move(src, dst);
            //}
        }
    }
}
