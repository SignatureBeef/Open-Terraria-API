using System;

namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Item
        {
            /// <summary>
            /// Occurs when NetDefaults is being executed
            /// Arg 1:  The item instance
            ///     2:  type
            /// </summary>
            public static Func<global::Terraria.Item, Int32, HookResult> PreNetDefaults;

            public static Action<global::Terraria.Item, Int32> PostNetDefaults;
        }
    }
}
