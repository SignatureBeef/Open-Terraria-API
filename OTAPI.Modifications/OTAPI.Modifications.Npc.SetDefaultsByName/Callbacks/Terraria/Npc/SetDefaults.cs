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
            var res = Hooks.Npc.PreSetDefaultsByName?.Invoke(npc, ref name);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(string) method.
        /// </summary>
        internal static void SetDefaultsByNameEnd(global::Terraria.NPC npc, ref string name) =>
            Hooks.Npc.PostSetDefaultsByName?.Invoke(npc, ref name);

        /// <summary>
        /// This method is injected into the start of the SetDefaults(int,float) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool SetDefaultsByIdBegin(global::Terraria.NPC npc, ref int type, ref float scaleOverride)
        {
            var res = Hooks.Npc.PreSetDefaultsById?.Invoke(npc, ref type, ref scaleOverride);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(int,float) method.
        /// </summary>
        internal static void SetDefaultsByIdEnd(global::Terraria.NPC npc, ref int type, ref float scaleOverride) =>
            Hooks.Npc.PostSetDefaultsById?.Invoke(npc, ref type, ref scaleOverride);
    }
}
