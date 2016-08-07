namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class World
		{
			#region Handlers
			public delegate HookResult DropMeteorHandler(ref int x, ref int y);
			#endregion

			public static DropMeteorHandler DropMeteor;
		}
	}
}
