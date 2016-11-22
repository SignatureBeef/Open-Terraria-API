namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Waterfall
	{
		internal static bool FindBegin(global::Terraria.WaterfallManager manager, ref bool forced) =>
			Hooks.Waterfall .PreFind?.Invoke(manager, ref forced) != HookResult.Cancel;

		internal static void FindEnd(global::Terraria.WaterfallManager manager, bool forced) => 
			Hooks.Waterfall.PostFind?.Invoke(manager, forced);
	}
}
