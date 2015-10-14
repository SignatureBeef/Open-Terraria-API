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
}
#endif