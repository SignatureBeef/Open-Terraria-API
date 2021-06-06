using System;
using System.IO;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;

namespace OTAPI.Client.Installer.Targets
{
    public class MacOSInstallTarget : BaseInstallTarget
    {
        public override string[] SearchPaths { get; } = new[]
        {
            "/Users/[USER_NAME]/Library/Application Support/Steam/steamapps/common/Terraria/Terraria.app/Contents/",
            "/Applications/Terraria.app/Contents/",
        };

        public override bool IsValidInstallPath(string folder)
        {
            bool valid = Directory.Exists(folder);

            var macOS = Path.Combine(folder, "MacOS");
            var resources = Path.Combine(folder, "Resources");

            var startScript = Path.Combine(macOS, "Terraria");
            var startBin = Path.Combine(macOS, "Terraria.bin.osx");
            var assembly = Path.Combine(resources, "Terraria.exe");

            valid &= Directory.Exists(macOS);
            valid &= Directory.Exists(resources);

            valid &= File.Exists(startScript);
            valid &= File.Exists(startBin);
            valid &= File.Exists(assembly);

            return valid;
        }

        private static void CopyFiles(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }

        public override void Install(string installPath)
        {
            var hostDir = "../../../../OTAPI.Client.Host/";

            var package = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "win7-x64" : (
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

            //var package = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            //    ? "win7-x64" : (
            //    RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx.10.11-x64" : "ubuntu.16.04-x64"
            //);

            var packagePath = Path.Combine(hostDir, "bin", "Debug", "net5.0", package, "publish");

            if (Directory.Exists(packagePath))
            {
                var otapiFolder = Path.Combine(installPath, "otapi");
                var sourceContentPath = Path.Combine(installPath, "Resources", "Content");
                var destContentPath = Path.Combine(otapiFolder, "Content");

                if (!Directory.Exists(otapiFolder))
                    Directory.CreateDirectory(otapiFolder);

                Console.WriteLine("Copying OTAPI...");
                CopyFiles(packagePath, otapiFolder);

                Console.WriteLine("Installing FNA libs...");
                InstallLibs(otapiFolder);

                Console.WriteLine("Installing LUA...");
                InstallLua(otapiFolder);

                Console.WriteLine("Copying Terraria Content files, this may take a while...");
                CopyFiles(sourceContentPath, destContentPath);

                Console.WriteLine("Patching launch scripts...");
                PatchOSXLaunch(installPath);

                Console.WriteLine("OSX install finished");
            }
            else
            {
                Console.Error.WriteLine("Failed to produce or find the appropriate package");
            }
        }

        void InstallLua(string otapiInstallPath)
        {
            var modificationsDir = Path.Combine(otapiInstallPath, "modifications");
            Directory.CreateDirectory(modificationsDir);
            TransferFile("ModFramework.Modules.Lua.dll", Path.Combine(modificationsDir, "ModFramework.Modules.Lua.dll"));

            var luaDir = Path.Combine(otapiInstallPath, "lua");
            Directory.CreateDirectory(luaDir);
            foreach (var lua in Directory.GetFiles("lua", "*.lua"))
                TransferFile(lua, Path.Combine(luaDir, Path.GetFileName(lua)));
        }

        void InstallLibs(string installPath)
        {
            var zipPath = DownloadZip("http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2");
            ExtractBZip2(zipPath, installPath);
        }

        void TransferFile(string src, string dst)
        {
            if (!File.Exists(src))
                throw new FileNotFoundException("Source binary not found, was it rebuilt before running the installer?\n" + src);

            if (File.Exists(dst))
                File.Delete(dst);

            File.Copy(src, dst);
        }

        string DownloadZip(string url)
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

        string ExtractBZip2(string zipPath, string dest = null)
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

        void PatchOSXLaunch(string installPath)
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
    }
}