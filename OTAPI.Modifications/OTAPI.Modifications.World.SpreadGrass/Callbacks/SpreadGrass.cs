namespace OTAPI.Callbacks.Terraria
{
	internal static partial class World
	{
		internal static bool SpreadGrass(ref int i, ref int j, ref int dirt, ref int grass, ref bool repeat, ref byte color)
		{
			var res = Hooks.World.SpreadGrass?.Invoke(ref i, ref j, ref dirt, ref grass, ref repeat, ref color);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}
