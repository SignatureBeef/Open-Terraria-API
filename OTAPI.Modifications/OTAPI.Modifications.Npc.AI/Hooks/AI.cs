namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Npc
		{
			#region Handlers
			public delegate HookResult PreAIHandler(global::Terraria.NPC npc);
			public delegate void PostAIHandler(global::Terraria.NPC npc);
			#endregion

			/// <summary>
			/// Occurs at the start of the AI() method.
			/// Arg 1:  The projectile instance
			/// </summary>
			public static PreAIHandler PreAI;

			/// <summary>
			/// Occurs when the AI() method ends
			/// Arg 1:  The projectile instance
			/// </summary>
			public static PostAIHandler PostAI;
		}
	}
}
