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

using System;
using System.IO;

namespace ModFramework.Modules.Lua
{
    public static class Hooks
    {
        public static ScriptManager ScriptManager { get; set; }

        [Modification(ModType.Runtime, "Loading Lua script interface")]
        public static void OnRunning()
        {
            Launch();
        }

        [Modification(ModType.Read, "Loading Lua script interface")]
        public static void OnModding(MonoMod.MonoModder modder)
        {
            Launch(modder);
        }

        static void Launch(MonoMod.MonoModder modder = null)
        {
            const string root = "lua";
            Directory.CreateDirectory(root);

            Console.WriteLine($"[LUA] Loading Lua scripts from ./{root}");

            ScriptManager = new ScriptManager(root, modder);
            ScriptManager.Initialise();
            ScriptManager.WatchForChanges();
        }
    }
}
