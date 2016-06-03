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
            if (Hooks.Item.PreNetDefaults != null)
                return Hooks.Item.PreNetDefaults(item, ref type) == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the NetDefaults method.
        /// </summary>
        internal static void NetDefaultsEnd(global::Terraria.Item item, ref int type)
        {
            if (Hooks.Item.PostNetDefaults != null)
                Hooks.Item.PostNetDefaults(item, ref type);
        }
    }
}
