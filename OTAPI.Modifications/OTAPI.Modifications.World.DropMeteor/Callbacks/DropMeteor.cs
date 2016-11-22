namespace OTAPI.Callbacks.Terraria
{
	internal static partial class WorldGen
	{
		internal static bool DropMeteor(ref int x, ref int y)
		{
			return Hooks.World.DropMeteor?.Invoke(ref x, ref y) == HookResult.Continue;
		}
	}
}
