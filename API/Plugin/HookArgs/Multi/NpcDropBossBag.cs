#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcDropBossBag
        {
            public MethodState State { get; set; }

            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Type { get; set; }
            public int Stack { get; set; }
            public bool NoBroadcast { get; set; }
            public int Prefix { get; set; }
            public bool NoGrabDelay { get; set; }
            public bool ReverseLookup { get; set; }
        }
    }
}
#endif