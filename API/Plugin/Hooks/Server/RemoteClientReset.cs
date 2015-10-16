#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct RemoteClientReset
        {
            public Terraria.RemoteClient Client { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.RemoteClientReset> RemoteClientReset = new HookPoint<HookArgs.RemoteClientReset>();
    }
}
#endif