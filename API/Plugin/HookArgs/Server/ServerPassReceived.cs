#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ServerPassReceived
        {
            public string Password { get; set; }
        }
    }
}
#endif