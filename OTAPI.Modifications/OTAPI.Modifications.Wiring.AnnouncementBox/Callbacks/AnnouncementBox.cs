namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Wiring
	{
		internal static bool AnnouncementBox(int x, int y, int signId)
		{
			return Hooks.Wiring.AnnouncementBox?.Invoke(x, y, signId) == HookResult.Continue;
		}
	}
}
