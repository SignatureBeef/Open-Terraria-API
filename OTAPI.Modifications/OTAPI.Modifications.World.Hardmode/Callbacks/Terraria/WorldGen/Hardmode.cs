namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class WorldGen
    {
        /// <summary>
        /// This method is injected into the start of the StartHardmode() method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool HardmodeBegin()
        {
            var res = Hooks.World.PreHardmode?.Invoke();
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the StartHardmode() method.
        /// </summary>
        internal static void HardmodeEnd() => Hooks.World.PostHardmode?.Invoke();
    }
}
