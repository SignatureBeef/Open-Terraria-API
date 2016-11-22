namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool UpdateBegin(global::Terraria.Player player, ref int i)
		{
			var res = Hooks.Player.PreUpdate?.Invoke(player, ref i);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void UpdateEnd(global::Terraria.Player player, int i) => Hooks.Player.PostUpdate?.Invoke(player, i);
	}
}
