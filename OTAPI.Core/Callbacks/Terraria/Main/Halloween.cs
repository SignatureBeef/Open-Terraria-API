namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// This method is ran right before the call to checkHalloween to allow
        /// cancelling of the function.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool Halloween()
        {
            var res = Hooks.Game.Halloween?.Invoke();
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }
    }
}
