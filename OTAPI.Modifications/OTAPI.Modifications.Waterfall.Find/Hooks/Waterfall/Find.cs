namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Waterfall
		{
			#region Handlers
			public delegate HookResult PreFindHandler(global::Terraria.WaterfallManager manager, ref bool forced);
			public delegate void PostFindHandler(global::Terraria.WaterfallManager manager, bool forced);
			#endregion

			public static PreFindHandler PreFind;

			public static PostFindHandler PostFind;
		}
	}
}
