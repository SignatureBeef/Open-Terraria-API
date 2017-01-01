using Terraria.World.Generation;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class World
		{
			#region Handlers
			public delegate HookResult GenerateHandler(ref int seed, ref GenerationProgress customProgressObject);
			#endregion

			public static GenerateHandler Generate;
		}
	}
}
