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

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.InvasionNpcSpawn> InvasionNpcSpawn = new HookPoint<HookArgs.InvasionNpcSpawn>();
    }
}
#endif