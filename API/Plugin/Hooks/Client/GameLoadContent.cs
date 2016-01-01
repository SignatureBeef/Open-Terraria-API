#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct GameLoadContent
        {
            public MethodState State { get; set; }

        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.GameLoadContent> GameLoadContent = new HookPoint<HookArgs.GameLoadContent>();
    }
}
#endif