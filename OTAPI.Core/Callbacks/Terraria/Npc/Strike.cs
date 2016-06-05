namespace OTAPI.Core.Callbacks.Terraria
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
            ref int cancelResult,
            ref int Damage,
            ref float knockBack,
            ref int hitDirection,
            ref bool crit,
            ref bool noEffect,
            ref bool fromNet
        )
        {
            if (Hooks.Npc.Strike != null)
                return Hooks.Npc.Strike(npc, ref cancelResult, ref Damage, ref knockBack, ref hitDirection, ref crit, ref noEffect, ref fromNet) == HookResult.Continue;
            return true;
        }
    }
}
