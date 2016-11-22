using Terraria.DataStructures;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool KillMeBegin(global::Terraria.Player player, ref PlayerDeathReason damageSource, ref double dmg, ref int hitDirection, ref bool pvp, ref string deathText)
		{
			var res = Hooks.Player.PreKillMe?.Invoke(player, ref damageSource, ref dmg, ref hitDirection, ref pvp, ref deathText);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void KillMeEnd(global::Terraria.Player player, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp, string deathText) =>
			Hooks.Player.PostKillMe?.Invoke(player, damageSource, dmg, hitDirection, pvp, deathText);
	}
}
