#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerAuthenticationChanged
        {
            public string AuthenticatedAs { get; set; }
            public string AuthenticatedBy { get; set; }
        }
    }
}
#endif