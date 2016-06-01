namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        internal static bool SetDefaultsByNameBegin(global::Terraria.NPC npc, string name)
        {
            if (Hooks.Npc.PreSetDefaultsByName != null)
                return Hooks.Npc.PreSetDefaultsByName(npc, name) == HookResult.Continue;
            return true;
        }

        internal static void SetDefaultsByNameEnd(global::Terraria.NPC npc, string name)
        {
            if (Hooks.Npc.PostSetDefaultsByName != null)
                Hooks.Npc.PostSetDefaultsByName(npc, name);
        }

        internal static bool SetDefaultsByIdBegin(global::Terraria.NPC npc, int type, float scaleOverride)
        {
            if (Hooks.Npc.PreSetDefaultsById != null)
                return Hooks.Npc.PreSetDefaultsById(npc, type, scaleOverride) == HookResult.Continue;
            return true;
        }

        internal static void SetDefaultsByIdEnd(global::Terraria.NPC npc, int type, float scaleOverride)
        {
            if (Hooks.Npc.PostSetDefaultsById != null)
                Hooks.Npc.PostSetDefaultsById(npc, type, scaleOverride);
        }
    }
}
