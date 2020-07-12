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
using System.Runtime.InteropServices;

namespace ReLogic.OS
{
    public abstract class patch_Platform
    {
        /** Begin Cross platform support - Determine platform based on RuntimeInformation rather than compile time */
        public static readonly Platform Current;

        public static extern void orig_ctor_Platform();
        [MonoMod.MonoModConstructor]
        static patch_Platform()
        {
            orig_ctor_Platform();
#if TerrariaServer_V1_4
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Current = new ReLogic.OS.OSX.OsxPlatform();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Current = new ReLogic.OS.Linux.LinuxPlatform();
            else
                Current = new ReLogic.OS.Windows.WindowsPlatform();
#endif
#if tModLoaderServer_V1_3
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Current = new ReLogic.OS.OsxPlatform();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Current = new ReLogic.OS.LinuxPlatform();
            else
                Current = new ReLogic.OS.WindowsPlatform();
#endif
        }
        /** End Cross platform support - Determine platform based on RuntimeInformation rather than compile time */
    }
}