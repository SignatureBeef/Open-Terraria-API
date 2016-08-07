namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Npc
		{
			#region Handlers
			public delegate HookResult PreTransformHandler(global::Terraria.NPC npc, ref int newType);
			public delegate void PostTransformHandler(global::Terraria.NPC npc);
			#endregion

			/// <summary>
			/// Occurs when an npc transforms
			/// Arg 1:	The npc instance
			///		2:	The new type that the npc will be transforming to
			/// </summary>
			public static PreTransformHandler PreTransform;

			/// <summary>
			/// Occurs when an npc transforms
			/// Arg 1: The npc instance
			/// </summary>
			public static PostTransformHandler PostTransform;
		}
	}
}
