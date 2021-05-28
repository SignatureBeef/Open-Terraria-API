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
using Mono.Cecil;
using OTAPI.Common;

namespace OTAPI.Client.Installer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("[OTAPI Client Install] NOTE: THIS IS NOT A REAL LAUNCHER. You must load Terraria yourself.");
            Console.WriteLine("[OTAPI Client Install] This program will install the required files to your client directory.");

            if (!File.Exists("OTAPI.Runtime.dll") || (new Func<bool>(() =>
             {
                 Console.Write("[OTAPI Client Install] Create runtime hooks now? n/[Y]: ");
                 return Console.ReadLine().ToLower() != "n";
             })).Invoke())
                CreateRuntimeEvents();

            var installPath = ClientHelpers.DetermineClientInstallPath();
            Console.WriteLine("[OTAPI Client Install] Copying to " + installPath);
            var hostSource = "../../../../OTAPI.Client.Host/bin/Debug/net472/";

            var files_from_host = new[]
            {
                Path.Combine(Environment.CurrentDirectory, "OTAPI.Runtime.dll"),
                Path.Combine(Environment.CurrentDirectory, "MonoMod.exe"),
                Path.Combine(Environment.CurrentDirectory, "MonoMod.Utils.dll"),
                Path.Combine(Environment.CurrentDirectory, "MonoMod.RuntimeDetour.dll"),
                "OTAPI.Client.Host.exe",
                "ModFramework.dll",
                "Mono.Cecil.dll",
                "NLua.dll",
                "KeraLua.dll",
                "liblua54.dylib",
                "liblua54.so",
                "lua54.dll",
            };

            foreach (var item in files_from_host)
            {
                var src = Path.IsPathRooted(item) ? item : Path.Combine(hostSource, item);
                var dst = Path.Combine(installPath, "Resources", Path.GetFileName(item));

                if (!File.Exists(src))
                    throw new FileNotFoundException("Host binary not found, was it rebuilt before running the launcher?\n" + src);

                if (File.Exists(dst))
                    File.Delete(dst);

                File.Copy(src, dst);
            }

            Console.WriteLine("[OTAPI Client Install] Patching launch scripts.");
            PatchOSXLaunch(installPath);

            Console.WriteLine("[OTAPI Client Install] Done.");
        }

        static void CreateRuntimeEvents()
        {
            Console.WriteLine("[OTAPI Client Install] Creating runtime events");
            var root = "../../../../OTAPI.Patcher/bin/Debug/net5.0/";

            using (var mm = new ModFramework.ModFwModder()
            {
                InputPath = Path.Combine(root, "OTAPI.exe"),
                //OutputPath = "OTAPI.dll",
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                //PublicEverything = true,

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            })
            {
                (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(Path.Combine(root, "EmbeddedResources"));
                //(mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(Path.GetDirectoryName(typeof(object).Assembly.Location));
                mm.Read();
                mm.MapDependencies();

                //mm.Log("[OTAPI Client Install] Generating OTAPI.Runtime.dll");
                var gen = new MonoMod.RuntimeDetour.HookGen.HookGenerator(mm, "OTAPI.Runtime.dll");
                using (ModuleDefinition mOut = gen.OutputModule)
                {
                    gen.Generate();

                    foreach (var asmref in mOut.AssemblyReferences.ToArray())
                    {
                        if (asmref.Name.Contains("System.Private.CoreLib") || asmref.Name.Contains("netstandard"))
                        {
                            //mOut.AssemblyReferences.Remove(asmref);
                        }
                    }

                    Directory.CreateDirectory("outputs");
                    mOut.Write("outputs/OTAPI.Runtime.dll");
                    ModFramework.Relinker.MscorlibRelinker.PostProcessMscorLib("outputs/OTAPI.Runtime.dll");
                }
            }
        }

        //static async Task InstallPackage(string packageId, string version, string folder)
        //{
        //    ILogger logger = NullLogger.Instance;
        //    CancellationToken cancellationToken = CancellationToken.None;

        //    SourceCacheContext cache = new SourceCacheContext();
        //    SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        //    FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

        //    NuGetVersion packageVersion = new NuGetVersion(version);
        //    using MemoryStream packageStream = new MemoryStream();

        //    await resource.CopyNupkgToStreamAsync(
        //        packageId,
        //        packageVersion,
        //        packageStream,
        //        cache,
        //        logger,
        //        cancellationToken);

        //    Console.WriteLine($"Downloaded package {packageId} {packageVersion}");

        //    using PackageArchiveReader packageReader = new PackageArchiveReader(packageStream);
        //    NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

        //    Console.WriteLine($"Tags: {nuspecReader.GetTags()}");
        //    Console.WriteLine($"Description: {nuspecReader.GetDescription()}");

        //    foreach (var file in packageReader.GetFiles())
        //    {
        //        packageReader.ExtractFile(file, Path.Combine(folder, file), logger);
        //    }
        //}

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

            // preserve the original files. the patcher will look for either of these
            // we need to rename these otherwise the runtime might accidentially load a Terraria.exe
            // when we actually use OTAPI.exe.
            // you typically notice this with missing field exceptions when you access
            // Terraria.versionNumber in the host application before terraria creates its
            // own assembly resolver (accessing main loads a bunch of other libs, so the
            // resolvers are important to use unless a custom one is made).
            {
                var src = Path.Combine(installPath, "Resources/Terraria.exe");
                var dst = Path.Combine(installPath, "Resources/Terraria.orig.exe");

                if (File.Exists(src))
                    File.Move(src, dst);
            }
            {
                var src = Path.Combine(installPath, "Resources/TerrariaServer.exe");
                var dst = Path.Combine(installPath, "Resources/TerrariaServer.orig.exe");

                if (File.Exists(src))
                    File.Move(src, dst);
            }
        }
    }
}
