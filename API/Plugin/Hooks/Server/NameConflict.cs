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

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NameConflict> NameConflict = new HookPoint<HookArgs.NameConflict>();
    }
}
#endif