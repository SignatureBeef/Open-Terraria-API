namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Chest
		{
			#region Handlers
			public delegate HookResult QuickStackHandler(int playerId, global::Terraria.Item item, int chestIndex);
			#endregion

			public static QuickStackHandler QuickStack;
		}
	}
}
