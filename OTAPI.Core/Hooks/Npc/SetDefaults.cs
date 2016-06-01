using System;

namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            /// <summary>
            /// Occurs when SetDefaults is being executed
            /// Arg 1:  The npc instance
            ///     2:  Item name
            /// </summary>
            public static Func<global::Terraria.NPC, String, HookResult> PreSetDefaultsByName;

            public static Action<global::Terraria.NPC, String> PostSetDefaultsByName;

            /// <summary>
            /// Occurs when SetDefaults is being executed
            /// Arg 1:  The npc instance
            ///     2:  Type
            ///     3:  noMatCheck flag
            /// </summary>
            public static Func<global::Terraria.NPC, Int32, Single, HookResult> PreSetDefaultsById;

            public static Action<global::Terraria.NPC, Int32, Single> PostSetDefaultsById;
        }
    }
}
