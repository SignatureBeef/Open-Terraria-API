using System;

namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            /// <summary>
            /// Occurs when NetDefaults is being executed
            /// Arg 1:  The npc instance
            ///     2:  type
            /// </summary>
            public static Func<global::Terraria.NPC, Int32, HookResult> PreNetDefaults;

            public static Action<global::Terraria.NPC, Int32> PostNetDefaults;
        }
    }
}
