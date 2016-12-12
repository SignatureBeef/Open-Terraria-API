namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Item
	{
		internal static bool CheckMaterialBegin(global::Terraria.Item item, ref bool result)
		{
			var res = Hooks.Item.PreCheckMaterial?.Invoke(item, ref result);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}
