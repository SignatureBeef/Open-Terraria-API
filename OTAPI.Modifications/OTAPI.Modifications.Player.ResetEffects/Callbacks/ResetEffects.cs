namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool ResetEffectsBegin(global::Terraria.Player player)
		{
			var res = Hooks.Player.PreResetEffects?.Invoke(player);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void ResetEffectsEnd(global::Terraria.Player player) =>
			Hooks.Player.PostResetEffects?.Invoke(player);
	}
}
