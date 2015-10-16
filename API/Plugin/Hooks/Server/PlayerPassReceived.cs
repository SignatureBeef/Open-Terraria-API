#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerPassReceived
        {
            public string Password { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PlayerPassReceived> PlayerPassReceived = new HookPoint<HookArgs.PlayerPassReceived>();
    }
}
#endif