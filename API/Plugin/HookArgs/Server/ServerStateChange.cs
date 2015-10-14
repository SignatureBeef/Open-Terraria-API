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
}
#endif