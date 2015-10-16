#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ServerStateChange
        {
            public ServerState ServerChangeState { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ServerStateChange> ServerStateChange = new HookPoint<HookArgs.ServerStateChange>();
    }
}
#endif