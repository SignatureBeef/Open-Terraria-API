namespace OTAPI.Core.Callbacks.Terraria
{
	internal static partial class RemoteClient
	{
		internal static bool PreReset(global::Terraria.RemoteClient remoteClient)
		{
			var result = Hooks.Net.RemoteClient.PreReset?.Invoke(remoteClient);
			if (result.HasValue) return result.Value == HookResult.Continue;
			return true;
		}

		internal static void PostReset(global::Terraria.RemoteClient remoteClient)
		{
			Hooks.Net.RemoteClient.PostReset?.Invoke(remoteClient);
		}
	}
}
