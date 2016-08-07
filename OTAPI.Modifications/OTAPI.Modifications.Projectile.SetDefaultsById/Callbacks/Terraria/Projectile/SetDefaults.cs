namespace OTAPI.Callbacks.Terraria
{
    internal static partial class Projectile
    {
        /// <summary>
        /// This method is injected into the start of the SetDefaults(int) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool SetDefaultsByIdBegin(global::Terraria.Projectile projectile, ref int Type)
        {
            var res = Hooks.Projectile.PreSetDefaultsById?.Invoke(projectile, ref Type);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the SetDefaults(int) method.
        /// </summary>
        internal static void SetDefaultsByIdEnd(global::Terraria.Projectile projectile, int Type) =>
            Hooks.Projectile.PostSetDefaultsById?.Invoke(projectile, Type);
    }
}
