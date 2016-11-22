namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate HookResult PreNetDefaultsHandler(global::Terraria.NPC npc, ref int type);
            public delegate void PostNetDefaultsHandler(global::Terraria.NPC npc, ref int type);
            #endregion

            /// <summary>
            /// Occurs at the start of the NetDefaults(int) method.
            /// Arg 1:  The npc instance
            ///     2:  Type
            /// </summary>
            public static PreNetDefaultsHandler PreNetDefaults;

            /// <summary>
            /// Occurs when the NetDefaults(int) method ends
            /// Arg 1:  The npc instance
            ///     2:  Type
            /// </summary>
            public static PostNetDefaultsHandler PostNetDefaults;
        }
    }
}
