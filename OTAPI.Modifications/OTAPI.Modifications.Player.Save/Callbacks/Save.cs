namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool SavePlayerBegin(global::Terraria.IO.PlayerFileData playerFile, ref bool skipMapSave)
		{
			var res = Hooks.Player.PreSave?.Invoke(playerFile, ref skipMapSave);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void SavePlayerEnd(global::Terraria.IO.PlayerFileData playerFile, bool skipMapSave) =>
			Hooks.Player.PostSave?.Invoke(playerFile, skipMapSave);
	}
}
