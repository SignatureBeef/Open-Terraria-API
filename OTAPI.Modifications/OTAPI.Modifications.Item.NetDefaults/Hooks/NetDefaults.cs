namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Item
        {
            #region Handlers
            public delegate HookResult PreNetDefaultsHandler(global::Terraria.Item item, ref int type);
            public delegate void PostNetDefaultsHandler(global::Terraria.Item item, ref int type);
            #endregion

            /// <summary>
            /// Occurs at the start of the NetDefaults(int) method.
            /// Arg 1:  The item instance
            ///     2:  Type
            /// </summary>
            public static PreNetDefaultsHandler PreNetDefaults;

            /// <summary>
            /// Occurs when the NetDefaults(int) method ends
            /// Arg 1:  The item instance
            ///     2:  Type
            /// </summary>
            public static PostNetDefaultsHandler PostNetDefaults;
        }
    }
}
