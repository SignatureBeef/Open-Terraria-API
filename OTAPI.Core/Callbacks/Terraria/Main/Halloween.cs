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
            if (Hooks.Game.Halloween != null)
                return Hooks.Game.Halloween() == HookResult.Continue;
            return true;
        }
    }
}
