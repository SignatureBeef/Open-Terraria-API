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
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using System;
using System.Reflection;
using ModFramework;
using ModFramework.Plugins;

/// <summary>
/// @doc Fixes platform issues in Terraria.Program.DisplayException
/// </summary>
/// <summary>
/// @doc Initiates the client module system
/// </summary>
namespace Terraria
{
    partial class patch_Program
    {
        // no WinForms access so just write to console
        public static extern void orig_DisplayException(Exception e);
        public static void DisplayException(Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        /// <summary>
        /// Triggers when mods should start attaching events. At this point assembly resolution should be ready on all platforms.
        /// </summary>
        public static event EventHandler OnLaunched;

#if Terraria // client

        public static extern void orig_LaunchGame(string[] args, bool monoArgs = false);
        public static void LaunchGame(string[] args, bool monoArgs = false)
        {
            Console.WriteLine($"[OTAPI] Starting up ({OTAPI.Common.Target}).");
            ModFramework.Plugins.PluginLoader.AssemblyLoader = new ModFramework.Plugins.LegacyAssemblyResolver();
            ModFramework.Plugins.PluginLoader.TryLoad();
            ModFramework.Modifier.Apply(ModFramework.ModType.Runtime);

            Main.versionNumber += " OTAPI";
            Main.versionNumber2 += " OTAPI";

            OnLaunched?.Invoke(null, EventArgs.Empty);

            orig_LaunchGame(args, monoArgs);
        }
#elif tModLoaderServer // tml
        public static extern void orig_LaunchGame_();
        public static void LaunchGame_()
        {
            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += ResolveDependency;

            PluginLoader.TryLoad();
            Console.WriteLine($"[OTAPI] Starting up ({OTAPI.Common.Target}).");
            Modifier.Apply(ModType.Runtime);

            OnLaunched?.Invoke(null, EventArgs.Empty);

            orig_LaunchGame_();
        }

        // replaces the AppDomain resolution already in Terraria.
        private static Assembly ResolveDependency(System.Runtime.Loader.AssemblyLoadContext ctx, AssemblyName assemblyName)
        {
            Console.WriteLine($"Looking for assembly: {assemblyName.Name}");
            var resourceName = assemblyName.Name + ".dll";
            var src = typeof(Program).Assembly;
            resourceName = Array.Find(src.GetManifestResourceNames(), element => element.EndsWith(resourceName));

            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                Console.WriteLine($"[OTAPI] Resolved ${resourceName}");
                using (var stream = src.GetManifestResourceStream(resourceName))
                    return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(stream);
            }

            return null;
        }
#else // server
        public static extern void orig_LaunchGame(string[] args, bool monoArgs = false);
        public static void LaunchGame(string[] args, bool monoArgs = false)
        {
            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += ResolveDependency;

            PluginLoader.TryLoad();
            Console.WriteLine($"[OTAPI] Starting up ({OTAPI.Common.Target}).");
            Modifier.Apply(ModType.Runtime);

            OnLaunched?.Invoke(null, EventArgs.Empty);

            orig_LaunchGame(args, monoArgs);
        }

        // replaces the AppDomain resolution already in Terraria.
        private static Assembly ResolveDependency(System.Runtime.Loader.AssemblyLoadContext ctx, AssemblyName assemblyName)
        {
            Console.WriteLine($"Looking for assembly: {assemblyName.Name}");
            var resourceName = assemblyName.Name + ".dll";
            var src = typeof(Program).Assembly;
            resourceName = Array.Find(src.GetManifestResourceNames(), element => element.EndsWith(resourceName));

            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                Console.WriteLine($"[OTAPI] Resolved ${resourceName}");
                using (var stream = src.GetManifestResourceStream(resourceName))
                    return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(stream);
            }

            return null;
        }
#endif
    }
}

// just a stub for rosyln - this is defined in another patch
namespace OTAPI
{
    [MonoMod.MonoModIgnore]
    public static partial class Common
    {
        public static readonly string Target;
    }
}