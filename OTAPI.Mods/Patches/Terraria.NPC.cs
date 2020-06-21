﻿/*
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

using OTAPI;

namespace Terraria
{
    class patch_NPC : Terraria.NPC
    {
        /** Begin Hook - SetDefaults */
        public extern void orig_SetDefaults(int Type, NPCSpawnParams spawnparams);
        public void SetDefaults(int Type, NPCSpawnParams spawnparams = default(NPCSpawnParams))
        {
            if (Hooks.NPC.SetDefaults?.Invoke(HookEvent.Before, this, ref Type, ref spawnparams, orig_SetDefaults) != HookResult.Cancel)
            {
                orig_SetDefaults(Type, spawnparams);
                Hooks.NPC.SetDefaults?.Invoke(HookEvent.After, this, ref Type, ref spawnparams, orig_SetDefaults);
            }
        }
        /** End Hook - SetDefaults */

        /** Begin Hook - UpdateNPC */
        public extern void orig_UpdateNPC(int i);
        public void UpdateNPC(int i)
        {
            if (Hooks.NPC.UpdateNPC?.Invoke(HookEvent.Before, this, ref i, orig_UpdateNPC) != HookResult.Cancel)
            {
                orig_UpdateNPC(i);
                Hooks.NPC.UpdateNPC?.Invoke(HookEvent.After, this, ref i, orig_UpdateNPC);
            }
        }
        /** End Hook - UpdateNPC */
    }
}