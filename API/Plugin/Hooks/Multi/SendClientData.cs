#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct SendClientData
        {
            public byte[] Data { get; set; }

            public int Offset { get; set; }

            public int Size { get; set; }

            public Terraria.Net.Sockets.SocketSendCallback Callback { get; set; }

            public object State { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.SendClientData> SendClientData = new HookPoint<HookArgs.SendClientData>();
    }
}
#endif