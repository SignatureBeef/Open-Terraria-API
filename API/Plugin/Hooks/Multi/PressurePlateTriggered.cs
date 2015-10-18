#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PressurePlateTriggered
        {
            public Terraria.Entity Sender { get; set; }

            public int X { get; set; }

            public int Y { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PressurePlateTriggered> PressurePlateTriggered = new HookPoint<HookArgs.PressurePlateTriggered>();
    }
}
#endif