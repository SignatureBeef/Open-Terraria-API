namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Item
		{
			#region Handlers
			public delegate HookResult PreCheckMaterialHandler(global::Terraria.Item item, ref bool result);
			#endregion

			/// <summary>
			/// Occurs at the start of the checkMat() method.
			/// Arg 1:  The item instance
			/// </summary>
			public static PreCheckMaterialHandler PreCheckMaterial;
		}
	}
}
