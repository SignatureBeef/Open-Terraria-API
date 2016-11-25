namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Chest
	{
		internal static bool QuickStack(int playerId, global::Terraria.Item item, int chestIndex)
		{
			var res = Hooks.Chest.QuickStack?.Invoke(playerId, item, chestIndex);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}