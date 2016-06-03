namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Item
        {
            #region Handlers
            public delegate HookResult PreSetDefaultsByNameHandler(global::Terraria.Item item, ref string name);
            public delegate void PostSetDefaultsByNameHandler(global::Terraria.Item item, ref string name);

            public delegate HookResult PreSetDefaultsByIdHandler(global::Terraria.Item item, ref int type, ref bool noMatCheck);
            public delegate void PostSetDefaultsByIdHandler(global::Terraria.Item item, ref int type, ref bool noMatCheck);
            #endregion

            /// <summary>
            /// Occurs at the start of the SetDefaults(string) method.
            /// Arg 1:  The item instance
            ///     2:  Item name
            /// </summary>
            public static PreSetDefaultsByNameHandler PreSetDefaultsByName;

            /// <summary>
            /// Occurs when the SetDefaults(string) method ends
            /// Arg 1:  The item instance
            ///     2:  Item name
            /// </summary>
            public static PostSetDefaultsByNameHandler PostSetDefaultsByName;

            /// <summary>
            /// Occurs at the start of the SetDefaults(int,bool) method
            /// Arg 1:  The item instance
            ///     2:  Type
            ///     3:  noMatCheck flag
            /// </summary>
            public static PreSetDefaultsByIdHandler PreSetDefaultsById;

            /// <summary>
            /// Occurs when the SetDefaults(int,bool) method ends
            /// Arg 1:  The item instance
            ///     2:  Type
            ///     3:  noMatCheck flag
            /// </summary>
            public static PostSetDefaultsByIdHandler PostSetDefaultsById;
        }
    }
}
