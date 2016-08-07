namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Npc
	{
		/// <summary>
		/// Called from Terraria.NPC.checkDead when an NPC has been killed
		/// </summary>
		/// <param name="npc"></param>
		internal static void Killed(global::Terraria.NPC npc) => Hooks.Npc.Killed?.Invoke(npc);
	}
}
