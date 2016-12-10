namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate HookResult StrikeHandler
            (
                global::Terraria.NPC npc,
                ref double cancelResult,
                ref int Damage,
                ref float knockBack,
                ref int hitDirection,
                ref bool crit,
                ref bool noEffect,
                ref bool fromNet,
				global::Terraria.Entity entity
            );
            #endregion

            /// <summary>
            /// Occurs at the start of the NetDefaults(int) method.
            /// Arg 1:  The npc instance
            ///     2:  Value to be returned if the hook cancelled the event
            ///     3:  damage
            ///     4:  knock back
            ///     5:  hit direction
            ///     6:  Type
            ///     7:  crit
            ///     8:  no effect
            ///     9:  from net
			///     10:	entity who may be striking the npc
            /// </summary>
            public static StrikeHandler Strike;
        }
    }
}
