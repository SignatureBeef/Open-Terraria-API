namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Item
		{
			#region Handlers
			public delegate HookResult PreUpdateHandler(global::Terraria.Item item, ref int i);
			public delegate void PostUpdateHandler(global::Terraria.Item item, int i);
			#endregion

			/// <summary>
			/// Occurs at the start of the UpdateItem method.
			/// Arg 1: item
			///		2: new item whoAmI value
			/// </summary>
			public static PreUpdateHandler PreUpdate;

			/// <summary>
			/// Occurs at the end of the UpdateItem method.
			/// Arg 1: item
			///		2: item whoAmI value
			/// </summary>
			public static PostUpdateHandler PostUpdate;
		}
	}
}
