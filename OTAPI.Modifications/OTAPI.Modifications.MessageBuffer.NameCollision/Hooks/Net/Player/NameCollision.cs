namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult NameCollisionHandler(global::Terraria.Player player);
			#endregion

			/// <summary>
			/// Occurs when a connecting player is about to be booted due to having the same name
			/// as a player whom is already playing.
			/// Arg 1: player instance
			/// </summary>
			public static NameCollisionHandler NameCollision;
		}
	}
}
