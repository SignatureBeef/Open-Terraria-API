#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PressurePlateTriggered
        {
            public OTA.Command.Sender Sender { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
#endif