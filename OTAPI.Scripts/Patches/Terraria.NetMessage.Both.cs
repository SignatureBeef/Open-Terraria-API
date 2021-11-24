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
#pragma warning disable CS0436 // Type conflicts with imported type

using System;
using OTAPI;
using Terraria.Localization;

/// <summary>
/// @doc Creates Hooks.NetMessage.SendData. Allows plugins to listen on packet requests.
/// </summary>
namespace Terraria
{
    partial class patch_NetMessage : Terraria.NetMessage
    {
#if TerrariaServer_SendDataNumber8
        public static extern void orig_SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0, float number8 = 0);

        public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0, float number8 = 0)
#else
        public static extern void orig_SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0);

        public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0)
#endif
        {
            var args = new Hooks.NetMessage.SendDataEventArgs()
            {
                Event = HookEvent.Before,
                MsgType = msgType,
                RemoteClient = remoteClient,
                IgnoreClient = ignoreClient,
                Text = text,
                Number = number,
                Number2 = number2,
                Number3 = number3,
                Number4 = number4,
                Number5 = number5,
                Number6 = number6,
                Number7 = number7,
            };

            if (Hooks.NetMessage.InvokeSendData(args) != HookResult.Cancel)
            {
                orig_SendData(args.MsgType, args.RemoteClient, args.IgnoreClient, args.Text, args.Number, args.Number2, args.Number3, args.Number4, args.Number5, args.Number6, args.Number7);
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
                public int BufferId { get; set; }
                public int MsgType { get; set; }
                public int RemoteClient { get; set; }
                public int IgnoreClient { get; set; }
                public NetworkText Text { get; set; }
                public int Number { get; set; }
                public float Number2 { get; set; }
                public float Number3 { get; set; }
                public float Number4 { get; set; }
                public int Number5 { get; set; }
                public int Number6 { get; set; }
                public int Number7 { get; set; }
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