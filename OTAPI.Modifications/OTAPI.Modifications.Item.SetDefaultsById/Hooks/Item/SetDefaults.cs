namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Item
        {
            #region Handlers
            public delegate HookResult PreSetDefaultsByIdHandler(global::Terraria.Item item, ref int type, ref bool noMatCheck);
            public delegate void PostSetDefaultsByIdHandler(global::Terraria.Item item, ref int type, ref bool noMatCheck);
            #endregion

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
