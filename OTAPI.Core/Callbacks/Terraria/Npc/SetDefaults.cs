namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        /// <summary>
        /// This method is injected into the start of the SetDefaults(string) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool SetDefaultsByNameBegin(global::Terraria.NPC npc, ref string name)
        {
            if (Hooks.Npc.PreSetDefaultsByName != null)
                return Hooks.Npc.PreSetDefaultsByName(npc, ref name) == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(string) method.
        /// </summary>
        internal static void SetDefaultsByNameEnd(global::Terraria.NPC npc, ref string name)
        {
            if (Hooks.Npc.PostSetDefaultsByName != null)
                Hooks.Npc.PostSetDefaultsByName(npc, ref name);
        }

        /// <summary>
        /// This method is injected into the start of the SetDefaults(int,float) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool SetDefaultsByIdBegin(global::Terraria.NPC npc, ref int type, ref float scaleOverride)
        {
            if (Hooks.Npc.PreSetDefaultsById != null)
                return Hooks.Npc.PreSetDefaultsById(npc, ref type, ref scaleOverride) == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(int,float) method.
        /// </summary>
        internal static void SetDefaultsByIdEnd(global::Terraria.NPC npc, ref int type, ref float scaleOverride)
        {
            if (Hooks.Npc.PostSetDefaultsById != null)
                Hooks.Npc.PostSetDefaultsById(npc, ref type, ref scaleOverride);
        }
    }
}
