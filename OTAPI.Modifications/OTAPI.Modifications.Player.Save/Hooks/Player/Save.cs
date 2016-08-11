namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreSaveHandler(global::Terraria.IO.PlayerFileData playerFile, ref bool skipMapSave);
			public delegate void PostSaveHandler(global::Terraria.IO.PlayerFileData playerFile, bool skipMapSave);
			#endregion

			/// <summary>
			/// Occurs at the start of the SavePlayer method.
			/// Arg 1: player data file
			///		2: skip map save
			/// </summary>
			public static PreSaveHandler PreSave;

			/// <summary>
			/// Occurs at the end of the SavePlayer method.
			/// Arg 1: player data file
			///		2: skip map save
			/// </summary>
			public static PostSaveHandler PostSave;
		}
	}
}
