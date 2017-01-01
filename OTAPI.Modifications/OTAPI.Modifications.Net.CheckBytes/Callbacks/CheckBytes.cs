namespace OTAPI.Callbacks.Terraria
{
	internal static partial class NetMessage
	{
		internal static bool CheckBytes(ref int bufferIndex)
		{
			var result = Hooks.Net.CheckBytes?.Invoke(ref bufferIndex);
			if (result.HasValue && result.Value == HookResult.Cancel)
				return false;

			return true;
		}
	}
}
