namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class World
		{
			#region Handlers
			public delegate HookResult StatueHandler(StatueType caller, float x, float y, int type, ref int num, ref int num2, ref int num3);
			#endregion

			/// <summary>
			/// Occurs when the game is to spawn a statue
			/// </summary>
			public static StatueHandler Statue;
		}
	}
}
