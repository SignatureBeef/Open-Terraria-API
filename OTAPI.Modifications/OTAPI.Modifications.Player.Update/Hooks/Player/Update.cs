namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreUpdateHandler(global::Terraria.Player player, ref int i);
			public delegate void PostUpdateHandler(global::Terraria.Player player, int i);
			#endregion

			/// <summary>
			/// Occurs at the start of the Update method.
			/// Arg 1: player
			///		2: new player whoAmI value
			/// </summary>
			public static PreUpdateHandler PreUpdate;

			/// <summary>
			/// Occurs at the end of the Update method.
			/// Arg 1: player
			///		2: player whoAmI value
			/// </summary>
			public static PostUpdateHandler PostUpdate;
		}
	}
}
