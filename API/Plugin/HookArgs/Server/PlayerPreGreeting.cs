#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerPreGreeting
        {
            public int Slot { get; set; }
            public string Motd { get; set; }
            public Microsoft.Xna.Framework.Color MotdColour { get; set; }
        }
    }
}
#endif