namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        internal static bool NetDefaultsBegin(global::Terraria.NPC npc, int type)
        {
            if (Hooks.Npc.PreNetDefaults != null)
                return Hooks.Npc.PreNetDefaults(npc, type) == HookResult.Continue;
            return true;
        }

        internal static void NetDefaultsEnd(global::Terraria.NPC npc, int type)
        {
            if (Hooks.Npc.PostNetDefaults != null)
                Hooks.Npc.PostNetDefaults(npc, type);
        }
    }
}
