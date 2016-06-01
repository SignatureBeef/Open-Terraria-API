namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Item
    {
        internal static bool SetDefaultsByNameBegin(global::Terraria.Item item, string itemName)
        {
            if (Hooks.Item.PreSetDefaultsByName != null)
                return Hooks.Item.PreSetDefaultsByName(item, itemName) == HookResult.Continue;
            return true;
        }

        internal static void SetDefaultsByNameEnd(global::Terraria.Item item, string itemName)
        {
            if (Hooks.Item.PostSetDefaultsByName != null)
                Hooks.Item.PostSetDefaultsByName(item, itemName);
        }

        internal static bool SetDefaultsByIdBegin(global::Terraria.Item item, int type, bool noMatCheck)
        {
            if (Hooks.Item.PreSetDefaultsById != null)
                return Hooks.Item.PreSetDefaultsById(item, type, noMatCheck) == HookResult.Continue;
            return true;
        }

        internal static void SetDefaultsByIdEnd(global::Terraria.Item item, int type, bool noMatCheck)
        {
            if (Hooks.Item.PostSetDefaultsById != null)
                Hooks.Item.PostSetDefaultsById(item, type, noMatCheck);
        }
    }
}
