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

namespace OTAPI
{
    public static partial class Hooks
    {
        public static class NPC
        {
#if TerrariaServer_V1_4
            public delegate HookResult SetDefaultsHandler(HookEvent @event, Terraria.NPC instance, ref int type, ref Terraria.NPCSpawnParams spawnparams, Action<int, Terraria.NPCSpawnParams> originalMethod);
            public static SetDefaultsHandler SetDefaults;

            public delegate HookResult UpdateNPCHandler(HookEvent @event, Terraria.NPC instance, ref int i, Action<int> originalMethod);
            public static UpdateNPCHandler UpdateNPC;

            public delegate HookResult DropLootHandler(HookEvent @event, Terraria.NPC instance, ref int itemIndex,
                ref int X, ref int Y, ref int Width, ref int Height, ref int Type,
                ref int Stack, ref bool noBroadcast, ref int pfix, ref bool noGrabDelay, ref bool reverseLookup
            );
            public static DropLootHandler DropLoot;
#endif

            public delegate void KilledHandler(Terraria.NPC instance);
            public static KilledHandler Killed;

            public delegate HookResult SpawnHandler(ref int index);
            public static SpawnHandler Spawn;
        }
    }
}
