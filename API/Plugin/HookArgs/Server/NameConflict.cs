#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NameConflict
        {
            public Terraria.Player Connectee { get; set; }
            public int BufferId { get; set; }
        }
    }
}
#endif