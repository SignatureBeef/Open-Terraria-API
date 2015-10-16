#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct MechSpawn
        {
            public float X { get; set; }

            public float Y { get; set; }

            public int Type { get; set; }

            public int Num { get; set; }

            public int Num2 { get; set; }

            public int Num3 { get; set; }

            public OTA.Callbacks.MechSpawnType Sender { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.MechSpawn> MechSpawn = new HookPoint<HookArgs.MechSpawn>();
    }
}
#endif