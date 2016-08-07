namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class World
		{
			#region Handlers
			public delegate HardmodeTileUpdateResult HardModeTileUpdateHandler(int x, int y, ref ushort type);
			#endregion

			public static HardModeTileUpdateHandler HardmodeTileUpdate;
		}
	}
}
