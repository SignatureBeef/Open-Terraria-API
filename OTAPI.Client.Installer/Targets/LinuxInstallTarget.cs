using OTAPI.Common;
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Client.Installer.Targets
{
    public class LinuxInstallTarget : LinuxInstallDiscoverer, IInstallTarget
    {
        public void Install(string installPath)
        {
            var packagePaths = this.PublishHostGame();

            if (packagePaths.Any())
            {
                var otapiFolder = Path.Combine(installPath, "otapi");
                var sourceContentPath = Path.Combine(installPath, "Content");
                var destContentPath = Path.Combine(otapiFolder, "Content");

                if (!Directory.Exists(otapiFolder))
                    Directory.CreateDirectory(otapiFolder);

                Console.WriteLine(Status = "Installing FNA libs...");
                this.InstallLibs(otapiFolder);

                Console.WriteLine(Status = "Installing LUA...");
                this.InstallLua(otapiFolder);

                Console.WriteLine(Status = "Installing ClearScript...");
                this.InstallClearScript(otapiFolder);

                Console.WriteLine(Status = "Installing extra files...");
                this.CopyInstallFiles(otapiFolder);

                Console.WriteLine(Status = "Installing Steamworks...");
                this.InstallSteamworks64(otapiFolder, installPath);

                Console.WriteLine(Status = "Copying Terraria Content files, this may take a while...");
                this.CopyFiles(sourceContentPath, destContentPath);

                Console.WriteLine(Status = "Patching launch scripts...");
                this.PatchLinuxLaunch(installPath);

                this.CopyOTAPI(otapiFolder, packagePaths);

                Console.WriteLine(Status = "Linux install finished");
            }
            else
            {
                Console.Error.WriteLine(Status = "Failed to produce or find the appropriate package");
            }
        }
    }
}