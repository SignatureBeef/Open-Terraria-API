namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool UpdateJumpHeightBegin(global::Terraria.Player player)
		{
			var res = Hooks.Player.PreUpdateJumpHeight?.Invoke(player);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void UpdateJumpHeightEnd(global::Terraria.Player player) => Hooks.Player.PostUpdateJumpHeight?.Invoke(player);
	}
}
