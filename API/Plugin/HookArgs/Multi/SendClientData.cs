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
}
#endif