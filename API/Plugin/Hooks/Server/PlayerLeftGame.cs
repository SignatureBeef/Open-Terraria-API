#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerLeftGame
        {
            public int Slot { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PlayerLeftGame> PlayerLeftGame = new HookPoint<HookArgs.PlayerLeftGame>();
    }
}
#endif