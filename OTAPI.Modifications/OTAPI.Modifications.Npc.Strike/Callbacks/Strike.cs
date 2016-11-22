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
            ref int cancelResult,
            ref int Damage,
            ref float knockBack,
            ref int hitDirection,
            ref bool crit,
            ref bool noEffect,
            ref bool fromNet
        )
        {
            var res = Hooks.Npc.Strike?.Invoke(npc, ref cancelResult, ref Damage, ref knockBack, ref hitDirection, ref crit, ref noEffect, ref fromNet);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }
    }
}
