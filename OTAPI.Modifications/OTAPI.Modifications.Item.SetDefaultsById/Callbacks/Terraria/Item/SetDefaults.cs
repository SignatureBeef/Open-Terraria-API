namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Item
    {
        /// <summary>
        /// This method is injected into the start of the SetDefaults(int,bool) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool SetDefaultsByIdBegin(global::Terraria.Item item, ref int type, ref bool noMatCheck)
        {
            var res = Hooks.Item.PreSetDefaultsById?.Invoke(item, ref type, ref noMatCheck);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(int,bool) method.
        /// </summary>
        internal static void SetDefaultsByIdEnd(global::Terraria.Item item, ref int type, ref bool noMatCheck) =>
            Hooks.Item.PostSetDefaultsById?.Invoke(item, ref type, ref noMatCheck);
    }
}
