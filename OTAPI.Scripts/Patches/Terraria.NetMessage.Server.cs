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
using Terraria.Localization;

namespace Terraria
{
    partial class patch_NetMessage : Terraria.NetMessage
    {
        public static extern void orig_SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0);

        public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {
            //Since we currently wrap the method we need to run these checks
            //as vanilla would have done this.
            if (Main.netMode == (int)OTAPI.NetMode.SinglePlayer)
                return;

            var bufferIndex = 256;
            if (Main.netMode == (int)OTAPI.NetMode.Server && remoteClient >= 0)
                bufferIndex = remoteClient;

            if (OTAPI.Hooks.NetMessage.SendData?.Invoke(HookEvent.Before, bufferIndex, ref msgType, ref remoteClient, ref ignoreClient, ref text, ref number, ref number2, ref number3, ref number4, ref number5, ref number6, ref number7) != HookResult.Cancel)
            {
                orig_SendData(msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
                OTAPI.Hooks.NetMessage.SendData?.Invoke(HookEvent.After, bufferIndex, ref msgType, ref remoteClient, ref ignoreClient, ref text, ref number, ref number2, ref number3, ref number4, ref number5, ref number6, ref number7);
            }
        }
    }
}

namespace OTAPI
{
    public enum NetMode : int
    {
        SinglePlayer = 0,
        Client = 1,
        Server = 2
    }

    public static partial class Hooks
    {
        public static partial class NetMessage
        {
            public delegate HookResult SendDataHandler
            (
                HookEvent @event,
                int bufferId,
                ref int msgType,
                ref int remoteClient,
                ref int ignoreClient,
                ref NetworkText text,
                ref int number,
                ref float number2,
                ref float number3,
                ref float number4,
                ref int number5,
                ref int number6,
                ref int number7
            );
            public static SendDataHandler SendData;
        }
    }
}
#endif
