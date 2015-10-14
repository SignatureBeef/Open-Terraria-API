#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcKilled
        {
            public int Type { get; set; }
            public int NetId { get; set; }
        }
    }
}
#endif