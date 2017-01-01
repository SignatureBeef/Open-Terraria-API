namespace OTAPI.Callbacks.Terraria
{
	internal static partial class World
	{
		internal static bool SpawnNpc()
		{
			var res = Hooks.World.SpawnNpc?.Invoke();
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}
