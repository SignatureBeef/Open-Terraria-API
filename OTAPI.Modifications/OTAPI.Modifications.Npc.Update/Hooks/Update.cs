namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Npc
		{
			#region Handlers
			public delegate HookResult PreUpdateHandler(global::Terraria.NPC npc, ref int i);
			public delegate void PostUpdateHandler(global::Terraria.NPC npc, int i);
			#endregion

			/// <summary>
			/// Occurs at the start of the UpdateNPC method.
			/// Arg 1: npc
			///		2: new npc whoAmI value
			/// </summary>
			public static PreUpdateHandler PreUpdate;

			/// <summary>
			/// Occurs at the end of the UpdateNPC method.
			/// Arg 1: npc
			///		2: npc whoAmI value
			/// </summary>
			public static PostUpdateHandler PostUpdate;
		}
    }
}
