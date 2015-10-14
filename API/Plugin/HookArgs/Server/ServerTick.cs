#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ServerTick
        {
            public static readonly ServerTick Empty = new ServerTick();
        }
    }
}
#endif