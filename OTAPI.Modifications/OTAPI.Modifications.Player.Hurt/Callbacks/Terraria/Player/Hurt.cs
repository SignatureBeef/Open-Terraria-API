namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool HurtBegin(ref double returnValue, global::Terraria.Player player, ref int Damage, ref int hitDirection, ref bool pvp, ref bool quiet, ref string deathText, ref bool Crit, ref int cooldownCounter)
		{
			var res = Hooks.Player.Hurt?.Invoke(ref returnValue, player, ref Damage, ref hitDirection, ref pvp, ref quiet, ref deathText, ref Crit, ref cooldownCounter);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}
