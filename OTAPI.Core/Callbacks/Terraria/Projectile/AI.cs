namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Projectile
    {
        /// <summary>
        /// This method is injected into the start of the AI() method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool AIBegin(global::Terraria.Projectile projectile)
        {
            var res = Hooks.Projectile.PreAI?.Invoke(projectile);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the AI() method.
        /// </summary>
        internal static void AIEnd(global::Terraria.Projectile projectile) => Hooks.Projectile.PostAI?.Invoke(projectile);
    }
}
