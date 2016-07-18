namespace OTAPI.Core
{
	public static partial class Hooks
	{
		public static partial class World
		{
			#region Handlers
			public delegate HookResult HardModeTileUpdateHandler(int x, int y, ref ushort type);
			#endregion

			public static HardModeTileUpdateHandler HardmodeTileUpdate;
		}
	}
}
