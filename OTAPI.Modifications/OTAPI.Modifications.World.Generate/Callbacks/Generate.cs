using Terraria.WorldBuilding;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class World
	{
		internal static bool Generate(ref int seed, ref GenerationProgress customProgressObject)
		{
			var res = Hooks.World.Generate?.Invoke(ref seed, ref customProgressObject);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}
