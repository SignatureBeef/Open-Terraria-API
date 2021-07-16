using OTAPI.Common;
using System;
using System.IO;

namespace OTAPI.Client.Installer.Targets
{
    public class MacOSInstallTarget : MacOSInstallDiscoverer, IInstallTarget
    {
        public void Install(string installPath)
        {
            var packagePath = this.PublishHostGame();

            if (Directory.Exists(packagePath))
            {
                var otapiFolder = Path.Combine(installPath, "otapi");
                var sourceContentPath = Path.Combine(installPath, "Resources", "Content");
                var destContentPath = Path.Combine(otapiFolder, "Content");

                if (!Directory.Exists(otapiFolder))
                    Directory.CreateDirectory(otapiFolder);

                Console.WriteLine(Status = "Copying OTAPI...");
                this.CopyFiles(packagePath, otapiFolder);

                Console.WriteLine(Status = "Installing FNA libs...");
                this.InstallLibs(otapiFolder);

                Console.WriteLine(Status = "Installing LUA...");
                this.InstallLua(otapiFolder);

                Console.WriteLine(Status = "Installing ClearScript...");
                this.InstallClearScript(otapiFolder);

                Console.WriteLine(Status = "Installing extra files...");
                this.CopyInstallFiles(otapiFolder);

                Console.WriteLine(Status = "Copying Terraria Content files, this may take a while...");
                this.CopyFiles(sourceContentPath, destContentPath);

                Console.WriteLine(Status = "Patching launch scripts...");
                this.PatchOSXLaunch(installPath);

                Console.WriteLine(Status = "OSX install finished");

                // TODO typings are not yet ready.
                //Console.Write("Would you like to generate TypeScript typings? y/n: ");
                //var resp = Console.ReadKey().Key;
                //Console.WriteLine();
                //if (resp == ConsoleKey.Y)
                //{
                //    Console.WriteLine("Generating typings...this will take a while");
                //    GenerateTypings(otapiFolder);
                //}

                //Console.WriteLine("Done");
            }
            else
            {
                Console.Error.WriteLine("Failed to produce or find the appropriate package");
            }
        }
    }
}