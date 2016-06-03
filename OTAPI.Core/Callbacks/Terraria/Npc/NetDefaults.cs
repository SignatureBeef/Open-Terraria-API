namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        /// <summary>
        /// This method is injected into the start of the NetDefaults method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool NetDefaultsBegin(global::Terraria.NPC npc, ref int type)
        {
            if (Hooks.Npc.PreNetDefaults != null)
                return Hooks.Npc.PreNetDefaults(npc, ref type) == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the NetDefaults method.
        /// </summary>
        internal static void NetDefaultsEnd(global::Terraria.NPC npc, ref int type)
        {
            if (Hooks.Npc.PostNetDefaults != null)
                Hooks.Npc.PostNetDefaults(npc, ref type);
        }
    }
}
