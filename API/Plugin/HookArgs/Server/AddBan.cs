#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct AddBan
        {
            public string RemoteAddress { get; set; }
        }
    }
}
#endif