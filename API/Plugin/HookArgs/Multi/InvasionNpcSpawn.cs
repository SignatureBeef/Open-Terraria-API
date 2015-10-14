#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct InvasionNpcSpawn
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
#endif