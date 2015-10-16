#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerAuthenticationChanging
        {
            public string AuthenticatedAs { get; set; }

            public string AuthenticatedBy { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PlayerAuthenticationChanging> PlayerAuthenticationChanging = new HookPoint<HookArgs.PlayerAuthenticationChanging>();
    }
}
#endif