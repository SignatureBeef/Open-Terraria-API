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

    public static partial class HookPoints
    {
        /// <summary>
        /// Server tick event. Occurs without players.
        /// </summary>
        public static readonly HookPoint<HookArgs.ServerTick> ServerTick = new HookPoint<HookArgs.ServerTick>();
    }
}
#endif