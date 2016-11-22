namespace OTAPI.Callbacks.Terraria
{
    internal static partial class Projectile
    {
        /// <summary>
        /// This method is injected into the start of the Update(int) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool UpdateBegin(global::Terraria.Projectile projectile, ref int i)
        {
            var res = Hooks.Projectile.PreUpdate?.Invoke(projectile, ref i);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the Update(int) method.
        /// </summary>
        internal static void UpdateEnd(global::Terraria.Projectile projectile, int i) =>
            Hooks.Projectile.PostUpdate?.Invoke(projectile, i);
    }
}
