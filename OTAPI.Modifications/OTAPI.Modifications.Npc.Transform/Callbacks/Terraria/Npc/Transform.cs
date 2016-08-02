namespace OTAPI.Core.Callbacks.Terraria
{
	internal static partial class Npc
	{
		internal static bool PreTransform(global::Terraria.NPC npc, ref int newType)
		{
			var result = Hooks.Npc.PreTransform?.Invoke(npc, ref newType);
			if (result == HookResult.Cancel)
				return false;

			return true;
		}

		internal static void PostTransform(global::Terraria.NPC npc) => Hooks.Npc.PostTransform?.Invoke(npc);
	}
}
