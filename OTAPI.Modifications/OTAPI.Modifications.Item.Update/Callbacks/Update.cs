namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Item
	{
		internal static bool UpdateBegin(global::Terraria.Item item, ref int i)
		{
			var res = Hooks.Item.PreUpdate?.Invoke(item, ref i);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void UpdateEnd(global::Terraria.Item item, int i) => Hooks.Item.PostUpdate?.Invoke(item, i);
	}
}
