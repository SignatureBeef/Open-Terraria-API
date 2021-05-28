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

using System;
using ModFramework;
using Terraria.Localization;
using OTAPI;
//using System.Windows.Forms;
using System.Threading;
using Steamworks;

namespace Terraria
{
    partial class patch_NetMessage : Terraria.NetMessage
    {
        public static extern void orig_SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0);

        public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {
            //Since we currently wrap the method we need to run these checks
            //as vanilla would have done this.
            if (Main.netMode == (int)NetMode.SinglePlayer)
                return;

            var bufferIndex = 256;
            if (Main.netMode == (int)NetMode.Server && remoteClient >= 0)
                bufferIndex = remoteClient;

            var args = new Hooks.NetMessage.SendDataEventArgs()
            {
                Event = HookEvent.Before,
                msgType = msgType,
                remoteClient = bufferIndex,
                ignoreClient = ignoreClient,
                text = text,
                number = number,
                number2 = number2,
                number3 = number3,
                number4 = number4,
                number5 = number5,
                number6 = number6,
                number7 = number7,
            };

            if (Hooks.NetMessage.InvokeSendData(args) != HookResult.Cancel)
            {
                orig_SendData(args.msgType, args.remoteClient, args.ignoreClient, args.text, args.number, args.number2, args.number3, args.number4, args.number5, args.number6, args.number7);
                args.Event = HookEvent.After;
                Hooks.NetMessage.InvokeSendData(args);
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
            public class SendDataEventArgs : EventArgs
            {
                public HookEvent Event { get; set; }
                public HookResult? Result { get; set; }
                public int bufferId { get; set; }
                public int msgType { get; set; }
                public int remoteClient { get; set; }
                public int ignoreClient { get; set; }
                public NetworkText text { get; set; }
                public int number { get; set; }
                public float number2 { get; set; }
                public float number3 { get; set; }
                public float number4 { get; set; }
                public int number5 { get; set; }
                public int number6 { get; set; }
                public int number7 { get; set; }
            }
            public static event EventHandler<SendDataEventArgs> SendData;

            public static HookResult? InvokeSendData(SendDataEventArgs args)
            {
                SendData?.Invoke(null, args);
                return args.Result;
            }
        }
    }
}