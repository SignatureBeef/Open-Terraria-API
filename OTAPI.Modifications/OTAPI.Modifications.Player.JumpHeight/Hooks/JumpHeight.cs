namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreUpdateJumpHeightHandler(global::Terraria.Player player);
			public delegate void PostUpdateJumpHeightHandler(global::Terraria.Player player);
			#endregion

			/// <summary>
			/// Occurs at the start of the UpdateJumpHeight method.
			/// Arg 1: player
			/// </summary>
			public static PreUpdateJumpHeightHandler PreUpdateJumpHeight;

			/// <summary>
			/// Occurs at the end of the UpdateJumpHeight method.
			/// Arg 1: player
			/// </summary>
			public static PostUpdateJumpHeightHandler PostUpdateJumpHeight;
		}
	}
}
