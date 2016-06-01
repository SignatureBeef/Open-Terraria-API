namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Item
    {
        internal static bool NetDefaultsBegin(global::Terraria.Item item, int type)
        {
            if (Hooks.Item.PreNetDefaults != null)
                return Hooks.Item.PreNetDefaults(item, type) == HookResult.Continue;
            return true;
        }

        internal static void NetDefaultsEnd(global::Terraria.Item item, int type)
        {
            if (Hooks.Item.PostNetDefaults != null)
                Hooks.Item.PostNetDefaults(item, type);
        }
    }
}
