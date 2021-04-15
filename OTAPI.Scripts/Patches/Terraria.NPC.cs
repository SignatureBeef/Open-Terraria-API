///*
//Copyright (C) 2020 DeathCradle

//This file is part of Open Terraria API v3 (OTAPI)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program. If not, see <http://www.gnu.org/licenses/>.
//*/
//#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
//#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

//using OTAPI;
//using ModFramework;
//using System;

//namespace Terraria
//{
//    class patch_NPC : Terraria.NPC
//    {
//#if !tModLoaderServer_V1_3
//        /** Begin Hook - SetDefaults */
//        public extern void orig_SetDefaults(int Type, NPCSpawnParams spawnparams);
//        public void SetDefaults(int Type, NPCSpawnParams spawnparams = default(NPCSpawnParams))
//        {
//            if (Hooks.NPC.SetDefaults?.Invoke(HookEvent.Before, this, ref Type, ref spawnparams, orig_SetDefaults) != HookResult.Cancel)
//            {
//                orig_SetDefaults(Type, spawnparams);
//                Hooks.NPC.SetDefaults?.Invoke(HookEvent.After, this, ref Type, ref spawnparams, orig_SetDefaults);
//            }
//        }
//        /** End Hook - SetDefaults */

//        /** Begin Hook - UpdateNPC */
//        public extern void orig_UpdateNPC(int i);
//        public void UpdateNPC(int i)
//        {
//            if (Hooks.NPC.UpdateNPC?.Invoke(HookEvent.Before, this, ref i, orig_UpdateNPC) != HookResult.Cancel)
//            {
//                orig_UpdateNPC(i);
//                Hooks.NPC.UpdateNPC?.Invoke(HookEvent.After, this, ref i, orig_UpdateNPC);
//            }
//        }
//        /** End Hook - UpdateNPC */
//#endif
//    }
//}
//namespace OTAPI
//{
//    public static partial class Hooks
//    {
//        public static partial class NPC
//        {
//#if !tModLoaderServer_V1_3
//            public delegate HookResult SetDefaultsHandler(HookEvent @event, Terraria.NPC instance, ref int type, ref Terraria.NPCSpawnParams spawnparams, Action<int, Terraria.NPCSpawnParams> originalMethod);
//            public static SetDefaultsHandler SetDefaults;

//            public delegate HookResult UpdateNPCHandler(HookEvent @event, Terraria.NPC instance, ref int i, Action<int> originalMethod);
//            public static UpdateNPCHandler UpdateNPC;
//#endif
//        }
//    }
//}

