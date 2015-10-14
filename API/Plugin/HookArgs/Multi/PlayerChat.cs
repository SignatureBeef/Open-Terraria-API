#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerChat
        {
            public string Message { get; set; }
            public Microsoft.Xna.Framework.Color Color { get; set; }
        }
    }
}
#endif