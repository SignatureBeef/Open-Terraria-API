namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate HookResult PreSetDefaultsByNameHandler(global::Terraria.NPC npc, ref string name);
            public delegate void PostSetDefaultsByNameHandler(global::Terraria.NPC npc, ref string name);
            #endregion

            /// <summary>
            /// Occurs at the start of the SetDefaults(string) method.
            /// Arg 1:  The npc instance
            ///     2:  Npc name
            /// </summary>
            public static PreSetDefaultsByNameHandler PreSetDefaultsByName;

            /// <summary>
            /// Occurs when the SetDefaults(string) method ends
            /// Arg 1:  The npc instance
            ///     2:  Npc name
            /// </summary>
            public static PostSetDefaultsByNameHandler PostSetDefaultsByName;
        }
    }
}
