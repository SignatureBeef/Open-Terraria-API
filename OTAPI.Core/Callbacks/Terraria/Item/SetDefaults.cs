namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Item
    {
        /// <summary>
        /// This method is injected into the start of the SetDefaults(string) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool SetDefaultsByNameBegin(global::Terraria.Item item, ref string itemName)
        {
            if (Hooks.Item.PreSetDefaultsByName != null)
                return Hooks.Item.PreSetDefaultsByName(item, ref itemName) == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(string) method.
        /// </summary>
        internal static void SetDefaultsByNameEnd(global::Terraria.Item item, ref string itemName)
        {
            if (Hooks.Item.PostSetDefaultsByName != null)
                Hooks.Item.PostSetDefaultsByName(item, ref itemName);
        }

        /// <summary>
        /// This method is injected into the start of the SetDefaults(int,bool) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool SetDefaultsByIdBegin(global::Terraria.Item item, ref int type, ref bool noMatCheck)
        {
            if (Hooks.Item.PreSetDefaultsById != null)
                return Hooks.Item.PreSetDefaultsById(item, ref type, ref noMatCheck) == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(int,bool) method.
        /// </summary>
        internal static void SetDefaultsByIdEnd(global::Terraria.Item item, ref int type, ref bool noMatCheck)
        {
            if (Hooks.Item.PostSetDefaultsById != null)
                Hooks.Item.PostSetDefaultsById(item, ref type, ref noMatCheck);
        }
    }
}
