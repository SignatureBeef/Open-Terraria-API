#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ServerUpdate
        {
            public static readonly ServerUpdate Begin = new ServerUpdate() { State = MethodState.Begin };
            public static readonly ServerUpdate End = new ServerUpdate() { State = MethodState.End };

            public MethodState State { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Game UpdateServer event. Does not occur without players.
        /// </summary>
        public static readonly HookPoint<HookArgs.ServerUpdate> ServerUpdate = new HookPoint<HookArgs.ServerUpdate>();
    }
}
#endif