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
using Microsoft.Xna.Framework;
using OTAPI;
using ModFramework;
using System;

namespace Terraria.Net
{
    partial class patch_NetManager : Terraria.Net.NetManager
    {
        /** Begin Hook - SendData */
        public extern void orig_SendData(Terraria.Net.Sockets.ISocket socket, Terraria.Net.NetPacket packet);
        public void SendData(Terraria.Net.Sockets.ISocket socket, Terraria.Net.NetPacket packet)
        {
            if (Hooks.Net.NetManager.SendData?.Invoke(HookEvent.Before, this, socket, ref packet, orig_SendData) != HookResult.Cancel)
            {
                orig_SendData(socket, packet);
                Hooks.Net.NetManager.SendData?.Invoke(HookEvent.After, this, socket, ref packet, orig_SendData);
            }
        }
        /** End Hook - SendData */
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            public static partial class NetManager
            {
                public delegate HookResult SendDataHandler(HookEvent @event, Terraria.Net.NetManager manager, Terraria.Net.Sockets.ISocket socket, ref Terraria.Net.NetPacket packet, Action<Terraria.Net.Sockets.ISocket, Terraria.Net.NetPacket> originalMethod);
                public static SendDataHandler SendData;
            }
        }
    }
}
