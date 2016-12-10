namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Npc
	{
		/// <summary>
		/// This method is injected into the StrikeNPC method along with an additional variable [cancelResult] which is returned
		/// if this method returns false.
		/// </summary>
		internal static bool Strike
		(
			global::Terraria.NPC npc,
			ref double cancelResult,
			ref int Damage,
			ref float knockBack,
			ref int hitDirection,
			ref bool crit,
			ref bool noEffect,
			ref bool fromNet,
			global::Terraria.Entity entity
		)
		{
			var res = Hooks.Npc.Strike?.Invoke(npc, ref cancelResult, ref Damage, ref knockBack, ref hitDirection, ref crit, ref noEffect, ref fromNet, entity);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}
