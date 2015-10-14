#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct SignTextSet
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int SignIndex { get; set; }
            public string Text { get; set; }
#if Full_API
            public Terraria.Sign OldSign { get; set; }
#endif
        }
    }
}
#endif