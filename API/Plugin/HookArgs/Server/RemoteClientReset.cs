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
}
#endif