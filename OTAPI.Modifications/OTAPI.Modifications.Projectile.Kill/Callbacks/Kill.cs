namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Projectile
	{
		internal static bool KillBegin(global::Terraria.Projectile projectile)
		{
			var res = Hooks.Projectile.PreKill?.Invoke(projectile);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void KillEnd(global::Terraria.Projectile projectile) =>
			Hooks.Projectile.PostKilled?.Invoke(projectile);
	}
}
