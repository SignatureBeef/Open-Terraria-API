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

                Console.WriteLine("Copying OTAPI...");
                this.CopyFiles(packagePath, otapiFolder);

                Console.WriteLine("Installing FNA libs...");
                this.InstallLibs(otapiFolder);

                Console.WriteLine("Installing LUA...");
                this.InstallLua(otapiFolder);

                Console.WriteLine("Installing ClearScript...");
                this.InstallClearScript(otapiFolder);

                Console.WriteLine("Copying Terraria Content files, this may take a while...");
                this.CopyFiles(sourceContentPath, destContentPath);

                Console.WriteLine("Patching launch scripts...");
                this.PatchOSXLaunch(installPath);

                Console.WriteLine("OSX install finished");

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