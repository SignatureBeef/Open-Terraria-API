using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;
using ModFramework.Modules.ClearScript.Typings;
using OTAPI.Common;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OTAPI.Client.Installer.Targets
{
    public class LinuxInstallTarget : LinuxInstallDiscoverer, IInstallTarget
    {
        public void Install(string installPath)
        {
            var packagePath = this.PublishHostGame();

            if (Directory.Exists(packagePath))
            {
                var otapiFolder = Path.Combine(installPath, "otapi");
                var sourceContentPath = Path.Combine(installPath, "Content");
                var destContentPath = Path.Combine(otapiFolder, "Content");

                if (!Directory.Exists(otapiFolder))
                    Directory.CreateDirectory(otapiFolder);

                Console.WriteLine("Copying OTAPI...");
                this.CopyFiles(packagePath, otapiFolder);

                Console.WriteLine("Installing FNA libs...");
                this.InstallLibs(otapiFolder);

                Console.WriteLine("Installing LUA...");
                this.InstallLua(otapiFolder);

                Console.WriteLine("Installing ClearScript...");
                this.InstallClearScript(otapiFolder);

                Console.WriteLine("Installing Steamworks...");
                this.InstallSteamworks64(otapiFolder, installPath);

                Console.WriteLine("Copying Terraria Content files, this may take a while...");
                this.CopyFiles(sourceContentPath, destContentPath);

                Console.WriteLine("Patching launch scripts...");
                this.PatchLinuxLaunch(installPath);

                Console.WriteLine("Linux install finished");
            }
            else
            {
                Console.Error.WriteLine("Failed to produce or find the appropriate package");
            }
        }
    }
}