#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerEnteringGame
        {
            public int Slot { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PlayerEnteringGame> PlayerEnteringGame = new HookPoint<HookArgs.PlayerEnteringGame>();
    }
}
#endif