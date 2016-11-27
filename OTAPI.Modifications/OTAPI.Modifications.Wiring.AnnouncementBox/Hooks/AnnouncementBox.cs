namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Wiring
		{
			#region Handlers
			public delegate HookResult AnnouncementBoxHandler(int x, int y, int signId);
			#endregion

			public static AnnouncementBoxHandler AnnouncementBox;
		}
	}
}
