using System;

namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Item
        {
            /// <summary>
            /// Occurs when SetDefaults is being executed
            /// Arg 1:  The item instance
            ///     2:  Item name
            /// </summary>
            public static Func<global::Terraria.Item, String, HookResult> PreSetDefaultsByName;

            public static Action PostSetDefaultsByName;

            /// <summary>
            /// Occurs when SetDefaults is being executed
            /// Arg 1:  The item instance
            ///     2:  Type
            ///     3:  noMatCheck flag
            /// </summary>
            public static Func<global::Terraria.Item, Int32, Boolean, HookResult> PreSetDefaultsById;

            public static Action PostSetDefaultsById;
        }
    }
}
