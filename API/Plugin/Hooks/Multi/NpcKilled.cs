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

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcKilled> NpcKilled = new HookPoint<HookArgs.NpcKilled>();
    }
}
#endif