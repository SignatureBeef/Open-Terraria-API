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

//using ModFramework;
//using System;

//namespace Terraria
//{
//    class patch_WorldGen : Terraria.WorldGen
//    {
//        private static extern void orig_hardUpdateWorld(int x, int y);
//        public static void hardUpdateWorld(int x, int y)
//        {
//            if (OTAPI.Hooks.WorldGen.HardUpdateWorld?.Invoke(HookEvent.Before, ref x, ref y, orig_hardUpdateWorld) != HookResult.Cancel)
//            {
//                orig_hardUpdateWorld(x, y);
//                OTAPI.Hooks.WorldGen.HardUpdateWorld?.Invoke(HookEvent.After, ref x, ref y, orig_hardUpdateWorld);
//            }
//        }
//    }
//}

//namespace OTAPI
//{
//    public static partial class Hooks
//    {
//        public static partial class WorldGen
//        {
//            public delegate HookResult HardUpdateWorldHandler(HookEvent @event, ref int x, ref int y, Action<int, int> originalMethod);
//            public static HardUpdateWorldHandler HardUpdateWorld;
//        }
//    }
//}

