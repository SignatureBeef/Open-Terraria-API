namespace OTAPI.Core
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreGreetHandler(ref int playerId);
			public delegate void PostGreetHandler(int playerId);
			#endregion

			/// <summary>
			/// Occurs at the start of the greetPlayer method.
			/// Arg 1: playerid
			/// </summary>
			public static PreGreetHandler PreGreet;

			/// <summary>
			/// Occurs at the end of the greetPlayer method.
			/// Arg 1: playerid
			/// </summary>
			public static PostGreetHandler PostGreet;
		}
	}
}
