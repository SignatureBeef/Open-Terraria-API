namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate HookResult PreDropLootHandler
			(
				global::Terraria.NPC npc,
				ref int itemId,
                ref int x,
                ref int y,
                ref int width,
                ref int height,
                ref int type,
                ref int stack,
                ref bool noBroadcast,
                ref int prefix,
                ref bool noGrabDelay,
                ref bool reverseLookup
            );
            public delegate void PostDropLootHandler
			(
				global::Terraria.NPC npc,
				int x,
                int y,
                int width,
                int height,
                int type,
                int stack,
                bool noBroadcast,
                int prefix,
                bool noGrabDelay,
                bool reverseLookup
            );
			#endregion

			/// <summary>
			/// Occurs at the start of the NetDefaults(int) method.
			/// Arg 1:  Npc that is dropping the loot
			///		2:  Item id
			///     3:  Type
			//      4:  x
			//      5:  y
			//      6:  width
			//      7:  height
			//      8:  type
			//      9:  stack
			//      10:  noBroadcast
			//      11: prefix
			//      12: noGrabDelay
			//      13: reverseLookup
			/// </summary>
			public static PreDropLootHandler PreDropLoot;

			/// <summary>
			/// Occurs when the NetDefaults(int) method ends
			/// Arg 1:  Npc that is dropping the loot
			///		2:  Type
			//      3:  x
			//      4:  y
			//      5:  width
			//      6:  height
			//      7:  type
			//      8:  stack
			//      9:  noBroadcast
			//      10: prefix
			//      11: noGrabDelay
			//      12: reverseLookup
			/// </summary>
			public static PostDropLootHandler PostDropLoot;
        }
    }
}
