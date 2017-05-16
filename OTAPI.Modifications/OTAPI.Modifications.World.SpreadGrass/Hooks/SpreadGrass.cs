namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class World
		{
			#region Handlers
			public delegate HookResult SpreadGrassHandler(ref int i, ref int j, ref int dirt, ref int grass, ref bool repeat, ref byte color);
			#endregion

			/// <summary>
			/// Occurs when the game is attempting to spread grass
			/// </summary>
			public static SpreadGrassHandler SpreadGrass;
		}
	}
}
