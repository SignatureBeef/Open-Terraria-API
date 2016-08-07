namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Npc
	{
		/// <summary>
		/// This method is injected into the start of the SetDefaults(int,bool) method.
		/// The return value will dictate if normal vanilla code should continue to run.
		/// </summary>
		/// <returns>True to continue on to vanilla code, otherwise false</returns>
		internal static bool SetDefaultsByIdBegin(global::Terraria.NPC npc, ref int type, ref float scaleOverride)
		{
			var res = Hooks.Npc.PreSetDefaultsById?.Invoke(npc, ref type, ref scaleOverride);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		/// <summary>
		/// This method is injected into the end of the SetDefaults(int,bool) method.
		/// </summary>
		internal static void SetDefaultsByIdEnd(global::Terraria.NPC npc, int type, float scaleOverride) =>
			Hooks.Npc.PostSetDefaultsById?.Invoke(npc, type, scaleOverride);
	}
}
