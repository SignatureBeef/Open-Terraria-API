namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate HookResult PreSetDefaultsByNameHandler(global::Terraria.NPC npc, ref string name);
            public delegate void PostSetDefaultsByNameHandler(global::Terraria.NPC npc, ref string name);

            public delegate HookResult PreSetDefaultsByIdHandler(global::Terraria.NPC npc, ref int type, ref float scaleOverride);
            public delegate void PostSetDefaultsByIdHandler(global::Terraria.NPC npc, ref int type, ref float scaleOverride);
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

            /// <summary>
            /// Occurs at the start of the SetDefaults(int,float) method.
            /// Arg 1:  The npc instance
            ///     2:  Type
            ///     3:  noMatCheck flag
            /// </summary>
            public static PreSetDefaultsByIdHandler PreSetDefaultsById;

            /// <summary>
            /// Occurs when the SetDefaults(int,float) method ends
            /// Arg 1:  The npc instance
            ///     2:  type
            /// </summary>
            public static PostSetDefaultsByIdHandler PostSetDefaultsById;
        }
    }
}
