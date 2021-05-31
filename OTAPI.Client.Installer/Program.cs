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
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;
using Mono.Cecil;
using OTAPI.Common;

namespace OTAPI.Client.Installer
{
    class Program
    {
        static void TransferFile(string src, string dst)
        {
            if (!File.Exists(src))
                throw new FileNotFoundException("Source binary not found, was it rebuilt before running the launcher?\n" + src);

            if (File.Exists(dst))
                File.Delete(dst);

            File.Copy(src, dst);
        }

        private static void CopyFiles(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("[OTAPI Client Install] NOTE: THIS IS NOT A REAL LAUNCHER. You must load Terraria yourself.");
            Console.WriteLine("[OTAPI Client Install] This program will install the required files to your client directory.");

            if (!File.Exists("OTAPI.Runtime.dll") || (new Func<bool>(() =>
             {
                 Console.Write("[OTAPI Client Install] Create runtime hooks now? n/[Y]: ");
                 var key = Console.ReadKey(true);
                 Console.WriteLine(key.Key);
                 return key.Key != ConsoleKey.N;
             })).Invoke())
                CreateRuntimeEvents();

            var installPath = ClientHelpers.DetermineClientInstallPath();
            var otapiInstallPath = Path.Combine(installPath, "otapi");
            var sourceContentPath = Path.Combine(installPath, "Resources", "Content");

            Console.WriteLine("[OTAPI Client Install] Using Terraria install " + installPath);
            var hostSource = "../../../../OTAPI.Client.Host/bin/Debug/net472";
            var luaSource = "../../../../ModFramework.Modules.Lua/bin/Debug/net472";


            Console.WriteLine("[OTAPI Client Install] Installing OTAPI to " + otapiInstallPath);
            Directory.CreateDirectory(otapiInstallPath);

            TransferFile("OTAPI.Runtime.dll", Path.Combine(otapiInstallPath, "OTAPI.Runtime.dll"));

            var modificationsDir = Path.Combine(otapiInstallPath, "modifications");
            var luaDir = Path.Combine(otapiInstallPath, "lua");
            var contentDir = Path.Combine(otapiInstallPath, "Content");
            Directory.CreateDirectory(modificationsDir);
            Directory.CreateDirectory(luaDir);

            Console.WriteLine("[OTAPI Client Install] Installing Lua");
            TransferFile("ModFramework.Modules.Lua.dll", Path.Combine(modificationsDir, "ModFramework.Modules.Lua.dll"));

            foreach (var lua in Directory.GetFiles("lua", "*.lua"))
                TransferFile(lua, Path.Combine(luaDir, Path.GetFileName(lua)));


            var files_from_host = new[]
            {
                "NLua.dll",
                "KeraLua.dll",
                "liblua54.dylib",
                "liblua54.so",
                "lua54.dll",
            };

            foreach (var item in files_from_host)
            {
                //var src = Path.IsPathRooted(item) ? item : Path.Combine(hostSource, item);
                var src = Path.Combine(luaSource, item);
                var dst = Path.Combine(otapiInstallPath, Path.GetFileName(item));

                TransferFile(src, dst);
            }


            if (!Directory.Exists(contentDir))
            {
                Console.WriteLine("[OTAPI Client Install] Copying Content, will take a bit...");
                CopyFiles(sourceContentPath, contentDir);
            }


            Console.WriteLine("[OTAPI Client Install] Installing host");
            CopyFiles(hostSource, otapiInstallPath);


            Console.WriteLine("[OTAPI Client Install] Installing osx libs");
            var osx = Path.Combine(otapiInstallPath, "osx");
            InstallLibs(osx);

            var CSteamworks_src = Path.Combine(installPath, "MacOS", "osx", "CSteamworks");
            var libsteam_api_src = Path.Combine(installPath, "MacOS", "osx", "libsteam_api.dylib");
            var steam_appid_src = Path.Combine(installPath, "MacOS", "steam_appid.txt");


            var CSteamworks_dst = Path.Combine(osx, "CSteamworks");
            var libsteam_api_dst = Path.Combine(osx, "libsteam_api.dylib");
            var steam_appid_dst = Path.Combine(otapiInstallPath, "steam_appid.txt");

            if (File.Exists(CSteamworks_src))
                TransferFile(CSteamworks_src, CSteamworks_dst);

            if (File.Exists(libsteam_api_src))
                TransferFile(libsteam_api_src, libsteam_api_dst);

            if (File.Exists(steam_appid_src))
                TransferFile(steam_appid_src, steam_appid_dst);

            //Console.WriteLine("[OTAPI Client Install] Testing");

            //var files_from_host = new[]
            //{
            //    "OTAPI.Runtime.dll",
            //    "MonoMod.exe",
            //    "MonoMod.Utils.dll",
            //    "MonoMod.RuntimeDetour.dll",
            //    Path.Combine(hostSource, "OTAPI.Client.Host.exe"),
            //    "ModFramework.dll",
            //    "Mono.Cecil.dll",
            //    "NLua.dll",
            //    "KeraLua.dll",
            //    "liblua54.dylib",
            //    "liblua54.so",
            //    "lua54.dll",
            //};

            //foreach (var item in files_from_host)
            //{
            //    //var src = Path.IsPathRooted(item) ? item : Path.Combine(hostSource, item);
            //    var src = item;
            //    var dst = Path.Combine(installPath, "Resources", Path.GetFileName(item));

            //    TransferFile(src, dst);
            //}

            Console.WriteLine("[OTAPI Client Install] Patching launch scripts.");
            PatchOSXLaunch(installPath);

            Console.WriteLine("[OTAPI Client Install] Done.");

            // TODO try and see how to get SDL working for dotnet 472. net5 would likely then work too, but also allow auto launch.
            //Environment.CurrentDirectory = otapiInstallPath;
            //var asm = System.Reflection.Assembly.LoadFile(Path.Combine(otapiInstallPath, "OTAPI.Client.Host.exe"));
            //asm.EntryPoint.Invoke(null, new object[] { new string[] { } });
 
        }



        static void InstallLibs(string installPath)
        {
            // http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2
            var zipPath = DownloadZip("http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2");
            var extr = ExtractZip(zipPath);

            Directory.CreateDirectory(installPath);

            var osx = Path.Combine(extr, "osx");
            foreach (var item in Directory.GetFiles(osx, "*"))
            {
                var src = item;
                var dst = Path.Combine(installPath, Path.GetFileName(item));

                if (File.Exists(dst)) File.Delete(dst);
                File.Copy(src, dst);
            }
        }

        public static string DownloadZip(string url)
        {
            Console.WriteLine($"[OTAPI.Client] Downloading {url}");
            var uri = new Uri(url);
            string filename = Path.GetFileName(uri.AbsolutePath);
            if (!String.IsNullOrWhiteSpace(filename))
            {
                var savePath = Path.Combine(Environment.CurrentDirectory, filename);

                if (!File.Exists(savePath))
                {
                    new System.Net.WebClient().DownloadFile(url, savePath);
                    //using var client = new HttpClient();
                    //var data = client.GetByteArrayAsync(url).Result;
                    //File.WriteAllBytes(savePath, data);
                }

                return savePath;
            }
            else throw new NotSupportedException();
        }

        public static string ExtractZip(string zipPath)
        {
            //var directory = Path.GetFileNameWithoutExtension(zipPath);
            //var info = new DirectoryInfo(directory);
            //Console.WriteLine($"[OTAPI.Client] Extracting to {directory}");

            //if (info.Exists) info.Delete(true);

            //info.Refresh();

            //if (!info.Exists || info.GetDirectories().Length == 0)
            //{

            var newname = Path.GetFileNameWithoutExtension(zipPath);

            using var raw = File.OpenRead(zipPath);
            using var ms = new MemoryStream();
            BZip2.Decompress(raw, ms, false);
            ms.Seek(0, SeekOrigin.Begin);
            newname = Path.GetFileNameWithoutExtension(newname);

            using var tarArchive = TarArchive.CreateInputTarArchive(ms, System.Text.Encoding.UTF8);


            if (Directory.Exists(newname))
                Directory.Delete(newname, true);

            Directory.CreateDirectory(newname);

            var abs = Path.GetFullPath(newname);
            tarArchive.ExtractContents(abs);
            tarArchive.Close();


            return newname;
        }

        static void TestLua()
        {
            Directory.CreateDirectory("lua");
            var sm = new ModFramework.Modules.Lua.ScriptManager("lua");
            sm.Initialise();

            Console.Write("Start file watcher? n/[Y]: ");
            var key = Console.ReadKey(true);
            Console.WriteLine(key.Key);
            if (key.Key != ConsoleKey.N)
            {
                sm.WatchForChanges();
                sm.Cli();
            }
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
                var patched = contents.Replace("./Terraria.bin.osx $@", "mono OTAPI.Client.Host.exe $@");
                patched = patched.Replace("export DYLD_LIBRARY_PATH", "cd ../otapi\n\texport DYLD_LIBRARY_PATH");
                if (contents != patched)
                {
                    File.WriteAllText(launch_script, patched);
                }
            }
            //{
            //    var launch_script = Path.Combine(installPath, "MacOS/Terraria");
            //    var backup_launch_script = Path.Combine(installPath, "MacOS/Terraria.bak.otapi");

            //    if (!File.Exists(backup_launch_script))
            //    {
            //        File.Copy(launch_script, backup_launch_script);
            //    }

            //    var contents = File.ReadAllText(launch_script);
            //    var patched = contents.Replace("./Terraria.bin.osx $@", "./OTAPI.Client.Host.bin.osx $@");

            //    if (contents != patched)
            //    {
            //        File.WriteAllText(launch_script, patched);
            //    }
            //}

            //{
            //    var bin = Path.Combine(installPath, "MacOS/Terraria.bin.osx");
            //    var patched_bin = Path.Combine(installPath, "MacOS/OTAPI.Client.Host.bin.osx");

            //    if (!File.Exists(patched_bin))
            //    {
            //        File.Copy(bin, patched_bin);
            //    }
            //}

            //// preserve the original files. the patcher will look for either of these
            //// we need to rename these otherwise the runtime might accidentially load a Terraria.exe
            //// when we actually use OTAPI.exe.
            //// you typically notice this with missing field exceptions when you access
            //// Terraria.versionNumber in the host application before terraria creates its
            //// own assembly resolver (accessing main loads a bunch of other libs, so the
            //// resolvers are important to use unless a custom one is made).
            //{
            //    var src = Path.Combine(installPath, "Resources/Terraria.exe");
            //    var dst = Path.Combine(installPath, "Resources/Terraria.orig.exe");

            //    if (File.Exists(src))
            //        File.Move(src, dst);
            //}
            //{
            //    var src = Path.Combine(installPath, "Resources/TerrariaServer.exe");
            //    var dst = Path.Combine(installPath, "Resources/TerrariaServer.orig.exe");

            //    if (File.Exists(src))
            //        File.Move(src, dst);
            //}
        }
    }
}
