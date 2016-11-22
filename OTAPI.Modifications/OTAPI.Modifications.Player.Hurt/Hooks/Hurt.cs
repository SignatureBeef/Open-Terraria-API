using Terraria.DataStructures;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreHurtHandler(ref double returnValue, global::Terraria.Player player, ref PlayerDeathReason damageSource, ref int Damage, ref int hitDirection, ref bool pvp, ref bool quiet, ref string deathText, ref bool Crit, ref int cooldownCounter);
			#endregion

			/// <summary>
			/// Occurs at the start of the ResetEffects method.
			/// Arg 1: player
			/// </summary>
			public static PreHurtHandler Hurt;
		}
	}
}
