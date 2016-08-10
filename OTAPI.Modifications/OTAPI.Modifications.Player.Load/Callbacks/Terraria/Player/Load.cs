namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Player
	{
		internal static bool LoadBegin(ref global::Terraria.IO.PlayerFileData data, ref string playerPath, ref bool cloudSave)
		{
			var res = Hooks.Player.PreLoad?.Invoke(ref data, ref playerPath, ref cloudSave);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void LoadEnd(string playerPath, bool cloudSave) =>
			Hooks.Player.PostLoad?.Invoke(playerPath, cloudSave);
	}
}
