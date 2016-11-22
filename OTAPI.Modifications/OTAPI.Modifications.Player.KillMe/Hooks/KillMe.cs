using Terraria.DataStructures;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreKillMeHandler(global::Terraria.Player player, ref PlayerDeathReason damageSource, ref double dmg, ref int hitDirection, ref bool pvp, ref string deathText);
			public delegate void PostKillMeHandler(global::Terraria.Player playerr, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp, string deathText);
			#endregion

			/// <summary>
			/// Occurs at the start of the KilMe method.
			/// Arg 1: player
			///		2: damage
			///		3: hit direction
			///		4: pvp
			///		5: death message
			/// </summary>
			public static PreKillMeHandler PreKillMe;

			/// <summary>
			/// Occurs at the end of the KilMe method.
			/// Arg 1: player
			///		2: damage
			///		3: hit direction
			///		4: pvp
			///		5: death message
			/// </summary>
			public static PostKillMeHandler PostKillMe;
		}
	}
}
