namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Item
    {
        /// <summary>
        /// This method is injected into the start of the NetDefaults method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool NetDefaultsBegin(global::Terraria.Item item, ref int type)
        {
            var res = Hooks.Item.PreNetDefaults?.Invoke(item, ref type);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the NetDefaults method.
        /// </summary>
        internal static void NetDefaultsEnd(global::Terraria.Item item, ref int type) =>
            Hooks.Item.PostNetDefaults?.Invoke(item, ref type);
    }
}
