#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct SignTextGet
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int SignIndex { get; set; }

            public string Text { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.SignTextGet> SignTextGet = new HookPoint<HookArgs.SignTextGet>();
    }
}
#endif