namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Npc
	{
		internal static bool UpdateBegin(global::Terraria.NPC npc, ref int i)
		{
			var res = Hooks.Npc.PreUpdate?.Invoke(npc, ref i);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void UpdateEnd(global::Terraria.NPC npc, int i) => Hooks.Npc.PostUpdate?.Invoke(npc, i);
	}
}
