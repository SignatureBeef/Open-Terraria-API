/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using Avalonia;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using System;

namespace OTAPI.Client.Launcher
{
    class Program
    {
        public static string LaunchFolder { get; set; } = Environment.CurrentDirectory;
        public static string LaunchID { get; set; }
        public static Targets.IPlatformTarget[] Targets = new Targets.IPlatformTarget[]
        {
            new Targets.MacOSPlatformTarget(),
            new Targets.WindowsPlatformTarget(),
            new Targets.LinuxPlatformTarget(),
        };

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            // start the launcher, then OTAPI if requested
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);

            if (LaunchID == "OTAPI")
                Actions.OTAPI.Launch(args);

            else if (LaunchID == "VANILLA")
                Actions.Vanilla.Launch(LaunchFolder, args);
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
