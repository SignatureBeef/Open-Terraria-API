namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult AnnounceHandler(int playerId);
			#endregion

			/// <summary>
			/// Occurs at the start of the greetPlayer method.
			/// Arg 1: playerid
			/// </summary>
			public static AnnounceHandler Announce;
		}
	}
}
