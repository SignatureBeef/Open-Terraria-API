using Terraria.DataStructures;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool KillMeBegin(global::Terraria.Player player, PlayerDeathReason damageSource, ref double dmg, ref int hitDirection, ref bool pvp)
		{
			var res = Hooks.Player.PreKillMe?.Invoke(player, damageSource, ref dmg, ref hitDirection, ref pvp);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void KillMeEnd(global::Terraria.Player player, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp) =>
			Hooks.Player.PostKillMe?.Invoke(player, damageSource, dmg, hitDirection, pvp);
	}
}
