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

using Avalonia;
using OTAPI.Common;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;

namespace OTAPI.Client.Installer
{
    class Program
    {
        public static Targets.IInstallTarget[] Targets = new Targets.IInstallTarget[]
        {
            new Targets.MacOSInstallTarget(),
            new Targets.WindowsInstallTarget(),
            new Targets.LinuxInstallTarget(),
        };

        public static void Cli()
        {
            Console.WriteLine("NOTE: THIS IS NOT A REAL LAUNCHER. You must load Terraria yourself.");
            Console.WriteLine("This program will install the required files to your client directory.");

            var installationPath = ClientHelpers.DetermineClientInstallPath(Targets);
            if (installationPath != null)
            {
                Console.WriteLine($"Installing to {installationPath.Path}");

                installationPath.Target.Install(installationPath.Path);
            }
            else
            {
                Console.Error.WriteLine("No install path found");
            }

            //Environment.CurrentDirectory = otapiInstallPath;
            //var asm = System.Reflection.Assembly.LoadFile(Path.Combine(otapiInstallPath, "OTAPI.Client.Host.exe"));
            //asm.EntryPoint.Invoke(null, new object[] { new string[] { } });
        }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            //if (args.Any(x => x == "--gui"))
            //{
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            //}
            //else
            //{
            //    Cli();
            //}
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .AfterSetup(AfterSetupCallback)
                .UsePlatformDetect()
                .LogToTrace();

        // Called after setup
        private static void AfterSetupCallback(AppBuilder appBuilder)
        {
            // Register icon provider(s)
            IconProvider.Register<FontAwesomeIconProvider>();
        }
    }
}
