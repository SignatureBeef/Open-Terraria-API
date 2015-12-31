#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct GameDraw { public MethodState State { get; set; } }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.GameDraw> GameDraw = new HookPoint<HookArgs.GameDraw>();
    }
}
#endif