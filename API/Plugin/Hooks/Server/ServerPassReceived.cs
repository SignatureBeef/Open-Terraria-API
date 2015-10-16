#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ServerPassReceived
        {
            public string Password { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ServerPassReceived> ServerPassReceived = new HookPoint<HookArgs.ServerPassReceived>();
    }
}
#endif