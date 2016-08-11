namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Main
	{
		internal static bool LoadContentBegin(global::Terraria.Main game)
		{
			var res = Hooks.Game.PreLoadContent?.Invoke(game);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void LoadContentEnd(global::Terraria.Main game) => Hooks.Game.PostLoadContent?.Invoke(game);
	}
}
