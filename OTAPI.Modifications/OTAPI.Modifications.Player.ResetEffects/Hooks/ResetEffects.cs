namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreResetEffectsHandler(global::Terraria.Player player);
			public delegate void PostResetEffectsHandler(global::Terraria.Player player);
			#endregion

			/// <summary>
			/// Occurs at the start of the ResetEffects method.
			/// Arg 1: player
			/// </summary>
			public static PreResetEffectsHandler PreResetEffects;

			/// <summary>
			/// Occurs at the end of the ResetEffects method.
			/// Arg 1: player
			/// </summary>
			public static PostResetEffectsHandler PostResetEffects;
		}
	}
}
