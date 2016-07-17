namespace OTAPI.Core
{
	public static partial class Hooks
	{
		public static partial class Npc
		{
			#region Handlers
			public delegate HookResult PreSetDefaultsByIdHandler(global::Terraria.NPC npc, ref int type, ref float scaleOverride);
			public delegate void PostSetDefaultsByIdHandler(global::Terraria.NPC npc, int type, float scaleOverride);
			#endregion

			/// <summary>
			/// Occurs at the start of the SetDefaults(int,float) method
			/// Arg 1:  The npc instance
			///     2:  Type
			///     3:  ScaleOverride
			/// </summary>
			public static PreSetDefaultsByIdHandler PreSetDefaultsById;

			/// <summary>
			/// Occurs when the SetDefaults(int,float) method ends
			/// Arg 1:  The npc instance
			///     2:  Type
			///     3:  ScaleOverride
			/// </summary>
			public static PostSetDefaultsByIdHandler PostSetDefaultsById;
		}
	}
}
