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
            var res = Hooks.Item.PreSetDefaultsByName?.Invoke(item, ref itemName);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(string) method.
        /// </summary>
        internal static void SetDefaultsByNameEnd(global::Terraria.Item item, ref string itemName) =>
            Hooks.Item.PostSetDefaultsByName?.Invoke(item, ref itemName);
    }
}
