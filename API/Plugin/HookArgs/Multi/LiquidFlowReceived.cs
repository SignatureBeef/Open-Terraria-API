#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct LiquidFlowReceived
        {
            public int X { get; set; }
            public int Y { get; set; }
            public byte Amount { get; set; }
            public bool Lava { get; set; }

            public bool Water
            {
                get { return !Lava; }
                set { Lava = !value; }
            }
        }
    }
}
#endif