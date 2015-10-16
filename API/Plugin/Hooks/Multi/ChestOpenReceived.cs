#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ChestOpenReceived
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int ChestIndex { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ChestOpenReceived> ChestOpenReceived = new HookPoint<HookArgs.ChestOpenReceived>();
    }
}
#endif