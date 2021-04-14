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

#if !tModLoaderServer_V1_3

using System;
using ModFramework;

namespace Terraria
{
    partial class patch_Netplay : Terraria.Netplay
    {
        private static extern void orig_StartServer();
        public static void StartServer()
        {
            if (OTAPI.Hooks.Netplay.StartServer?.Invoke(HookEvent.Before, orig_StartServer) != HookResult.Cancel)
            {
                orig_StartServer();
                OTAPI.Hooks.Netplay.StartServer?.Invoke(HookEvent.After, orig_StartServer);
            }
        }
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Netplay
        {
            public delegate HookResult StartServerHandler(HookEvent @event, Action originalMethod);
            public static StartServerHandler StartServer;
        }
    }
}

#endif
