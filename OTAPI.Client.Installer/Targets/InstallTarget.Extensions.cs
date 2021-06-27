using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;
using ModFramework.Modules.ClearScript.Typings;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OTAPI.Client.Installer.Targets
{
    public static class InstallTargetExtensions
    {
        public static string PublishHostGame(this IInstallTarget target)
        {
            var hostDir = "../../../../OTAPI.Client.Host/";

            var package = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "win-x64" : (
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx.10.11-x64" : "ubuntu.16.04-x64"
            );

            using var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "dotnet",
                    Arguments = "publish -r " + package,
                    //Arguments = "msbuild -restore -t:PublishAllRids",
                    WorkingDirectory = hostDir
                },
            };
            process.Start();
            process.WaitForExit();

            Console.WriteLine("Published");

            return Path.Combine(hostDir, "bin", "Debug", "net5.0", package, "publish");
        }

        public static string PublishHostLauncher(this IInstallTarget target)
        {
            var hostDir = "../../../../OTAPI.Client.Launcher/";

            var package = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "win-x64" : (
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx.10.11-x64" : "ubuntu.16.04-x64"
            );

            using var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "dotnet",
                    Arguments = $"publish -r {package} --framework net5.0 -p:PublishTrimmed=true -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true -c Release",
                    //Arguments = "msbuild -restore -t:PublishAllRids",
                    WorkingDirectory = hostDir
                },
            };
            process.Start();
            process.WaitForExit();

            Console.WriteLine("Published");

            return Path.Combine(hostDir, "bin", "Release", "net5.0", package, "publish");
        }

        public static void CopyFiles(this IInstallTarget target, string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }

        public static void TransferFile(this IInstallTarget target, string src, string dst)
        {
            if (!File.Exists(src))
                throw new FileNotFoundException("Source binary not found, was it rebuilt before running the installer?\n" + src);

            if (File.Exists(dst))
                File.Delete(dst);

            File.Copy(src, dst);
        }

        public static string DownloadZip(this IInstallTarget target, string url)
        {
            Console.WriteLine($"Downloading {url}");
            var uri = new Uri(url);
            string filename = Path.GetFileName(uri.AbsolutePath);
            if (!String.IsNullOrWhiteSpace(filename))
            {
                var savePath = Path.Combine(Environment.CurrentDirectory, filename);
                var info = new FileInfo(savePath);

                //if (info.Exists) info.Delete();

                if (!info.Exists || info.Length == 0)
                {
                    using (var wc = new System.Net.WebClient())
                    {
                        int lastPercentage = -1;
                        wc.DownloadProgressChanged += (s, e) =>
                        {
                            if (lastPercentage != e.ProgressPercentage)
                            {
                                lastPercentage = e.ProgressPercentage;
                                Console.WriteLine($"Downloading fnalibs...{e.ProgressPercentage}%");
                            }
                        };
                        wc.DownloadFileTaskAsync(new Uri(url), savePath).Wait();
                    }
                }

                return savePath;
            }
            else throw new NotSupportedException();
        }

        public static string ExtractBZip2(this IInstallTarget target, string zipPath, string dest = null)
        {
            using var raw = File.OpenRead(zipPath);
            using var ms = new MemoryStream();
            BZip2.Decompress(raw, ms, false);
            ms.Seek(0, SeekOrigin.Begin);

            using var tarArchive = TarArchive.CreateInputTarArchive(ms, System.Text.Encoding.UTF8);

            var abs = Path.GetFullPath(dest);
            tarArchive.ExtractContents(abs);
            tarArchive.Close();

            return dest;
        }

        public static void GenerateTypings(this IInstallTarget target, string rootFolder)
        {
            var patcherDir = "../../../../OTAPI.Patcher/";

            using (var typeGen = new TypingsGenerator())
            {
                AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
                {
                    var asr = new AssemblyName(e.Name);
                    var exe = Path.Combine(rootFolder, $"{asr.Name}.exe");
                    var dll = Path.Combine(rootFolder, $"{asr.Name}.dll");

                    if (File.Exists(exe))
                        return Assembly.LoadFile(exe);

                    if (File.Exists(dll))
                        return Assembly.LoadFile(dll);

                    exe = Path.Combine(patcherDir, "bin", "Debug", "net5.0", "EmbeddedResources", $"{asr.Name}.exe");
                    dll = Path.Combine(patcherDir, "bin", "Debug", "net5.0", "EmbeddedResources", $"{asr.Name}.dll");

                    if (File.Exists(exe))
                        return Assembly.LoadFile(exe);

                    if (File.Exists(dll))
                        return Assembly.LoadFile(dll);

                    return null;
                };

                //typeGen.AddAssembly(typeof(Mono.Cecil.AssemblyDefinition).Assembly);

                var otapi = Path.Combine(rootFolder, "OTAPI.exe");
                var otapiRuntime = Path.Combine(rootFolder, "OTAPI.Runtime.dll");

                if (File.Exists(otapi))
                    typeGen.AddAssembly(Assembly.LoadFile(otapi));

                //if (File.Exists(otapiRuntime))
                //    typeGen.AddAssembly(Assembly.LoadFile(otapiRuntime));

                //var outDir = Path.Combine(rootFolder, "clearscript", "typings");
                var outDir = Path.Combine(rootFolder, "clearscript", "test", "src", "typings");
                typeGen.Write(outDir);

                File.WriteAllText(Path.Combine(outDir, "index.js"), "// typings only\n");
            }
        }

        public static void InstallClearScript(this IInstallTarget target, string otapiInstallPath)
        {
            var modificationsDir = Path.Combine(otapiInstallPath, "modifications");
            Directory.CreateDirectory(modificationsDir);
            target.TransferFile("ModFramework.Modules.ClearScript.dll", Path.Combine(modificationsDir, "ModFramework.Modules.ClearScript.dll"));

            var csDir = Path.Combine(otapiInstallPath, "clearscript");
            Directory.CreateDirectory(csDir);
            foreach (var file in Directory.GetFiles("clearscript", "*.js"))
                target.TransferFile(file, Path.Combine(csDir, Path.GetFileName(file)));
        }

        public static void InstallLua(this IInstallTarget target, string otapiInstallPath)
        {
            var modificationsDir = Path.Combine(otapiInstallPath, "modifications");
            Directory.CreateDirectory(modificationsDir);
            target.TransferFile("ModFramework.Modules.Lua.dll", Path.Combine(modificationsDir, "ModFramework.Modules.Lua.dll"));

            var luaDir = Path.Combine(otapiInstallPath, "lua");
            Directory.CreateDirectory(luaDir);
            foreach (var lua in Directory.GetFiles("lua", "*.lua"))
                target.TransferFile(lua, Path.Combine(luaDir, Path.GetFileName(lua)));
        }

        public static void InstallLibs(this IInstallTarget target, string installPath)
        {
            var zipPath = target.DownloadZip("http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2");
            target.ExtractBZip2(zipPath, installPath);
        }

        public static void InstallSteamworks64(this IInstallTarget target, string installPath, string vanillaInstall)
        {
            var zipPath = target.DownloadZip("https://github.com/rlabrecque/Steamworks.NET/releases/download/15.0.1/Steamworks.NET-Standalone_15.0.1.zip");
            var folderName = Path.GetFileNameWithoutExtension(zipPath);
            if (Directory.Exists(folderName)) Directory.Delete(folderName, true);
            ZipFile.ExtractToDirectory(zipPath, folderName);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                target.CopyFiles(Path.Combine(folderName, "Windows-x64"), installPath);
            else
                target.CopyFiles(Path.Combine(folderName, "OSX-Linux-x64"), installPath);

            // ensure to use terrarias steam appid
            target.TransferFile(Path.Combine(vanillaInstall, "steam_appid.txt"), Path.Combine(installPath, "steam_appid.txt"));
        }

        public static void PatchOSXLaunch(this IInstallTarget target, string installPath)
        {
            var launch_script = Path.Combine(installPath, "MacOS/Terraria");
            var backup_launch_script = Path.Combine(installPath, "MacOS/Terraria.bak.otapi");

            if (!File.Exists(backup_launch_script))
            {
                File.Copy(launch_script, backup_launch_script);

                var contents = File.ReadAllText(launch_script);
                var patched = contents.Replace("./Terraria.bin.osx $@", "./OTAPI.Client.Host $@");
                patched = patched.Replace("export DYLD_LIBRARY_PATH", "cd ../otapi\n\texport DYLD_LIBRARY_PATH");
                if (contents != patched)
                {
                    File.WriteAllText(launch_script, patched);
                }
            }
        }

        public static void PatchWindowsLaunch(this IInstallTarget target, string installPath)
        {
            var launch_file = Path.Combine(installPath, "Terraria.exe");

            // backup Terraria.exe to Terraria.orig.exe
            {
                var backup_launch_file = Path.Combine(installPath, "Terraria.orig.exe");

                if (!File.Exists(backup_launch_file))
                {
                    File.Copy(launch_file, backup_launch_file);
                }
            }

            // publish and copy OTAPI.Client.Launcher
            {
                var output = target.PublishHostLauncher();
                var launcher = Path.Combine(output, "Terraria.exe");

                if (!File.Exists(launch_file)) 
                    throw new Exception($"Failed to produce launcher to: {launcher}");

                if (File.Exists(launch_file))
                    File.Delete(launch_file);

                File.Copy(Path.Combine(output, "Terraria.exe"), launch_file);
            }
        }
    }
}
