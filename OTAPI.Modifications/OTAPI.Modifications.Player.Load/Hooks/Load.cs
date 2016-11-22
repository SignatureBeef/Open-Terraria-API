namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Player
		{
			#region Handlers
			public delegate HookResult PreLoadHandler(ref global::Terraria.IO.PlayerFileData data, ref string playerPath, ref bool cloudSave);
			public delegate void PostLoadHandler(string playerPath, bool cloudSave);
			#endregion

			/// <summary>
			/// Occurs at the start of the SavePlayer method.
			/// Arg 1: player data file path
			///		2: cloud save
			/// </summary>
			public static PreLoadHandler PreLoad;

			/// <summary>
			/// Occurs at the end of the SavePlayer method.
			/// Arg 1: player data file path
			///		2: cloud save
			/// </summary>
			public static PostLoadHandler PostLoad;
		}
	}
}
