using Terraria.DataStructures;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreKillMeHandler(global::Terraria.Player player, PlayerDeathReason damageSource, ref double dmg, ref int hitDirection, ref bool pvp);
			public delegate void PostKillMeHandler(global::Terraria.Player playerr, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp);
			#endregion

			/// <summary>
			/// Occurs at the start of the KillMe method.
			/// Arg 1: player
			///		2: damage
			///		3: hit direction
			///		4: pvp
			/// </summary>
			public static PreKillMeHandler PreKillMe;

			/// <summary>
			/// Occurs at the end of the KillMe method.
			/// Arg 1: player
			///		2: damage
			///		3: hit direction
			///		4: pvp
			/// </summary>
			public static PostKillMeHandler PostKillMe;
		}
	}
}
