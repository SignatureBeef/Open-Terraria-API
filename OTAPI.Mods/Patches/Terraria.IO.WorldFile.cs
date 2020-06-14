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
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
using OTAPI;

namespace Terraria
{
    public class patch_WorldFile
    {
        public static extern void orig_loadWorld(bool loadFromCloud);
        public static void loadWorld(bool loadFromCloud)
        {
            if (Hooks.IO.WorldFile.LoadWorld?.Invoke(HookEvent.Pre, ref loadFromCloud, orig_loadWorld) != HookResult.Cancel)
            {
                orig_loadWorld(loadFromCloud);
                Hooks.IO.WorldFile.LoadWorld?.Invoke(HookEvent.Post, ref loadFromCloud, orig_loadWorld);
            }
        }

        public static extern void orig_saveWorld(bool useCloudSaving, bool resetTime = false);
        public static void saveWorld(bool useCloudSaving, bool resetTime = false)
        {
            if (Hooks.IO.WorldFile.SaveWorld?.Invoke(HookEvent.Pre, ref useCloudSaving, ref resetTime, orig_saveWorld) != HookResult.Cancel)
            {
                orig_saveWorld(useCloudSaving, resetTime);
                Hooks.IO.WorldFile.SaveWorld?.Invoke(HookEvent.Post, ref useCloudSaving, ref resetTime, orig_saveWorld);
            }
        }
    }
}
