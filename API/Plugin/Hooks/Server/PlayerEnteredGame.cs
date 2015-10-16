#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerEnteredGame
        {
            public int Slot { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PlayerEnteredGame> PlayerEnteredGame = new HookPoint<HookArgs.PlayerEnteredGame>();
    }
}
#endif