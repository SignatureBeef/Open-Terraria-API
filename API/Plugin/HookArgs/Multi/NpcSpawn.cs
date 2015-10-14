#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcSpawn
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Type { get; set; }
            public int Start { get; set; }
        }
    }
}
#endif