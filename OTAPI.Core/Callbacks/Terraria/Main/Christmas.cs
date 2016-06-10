namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// This method is ran right before the call to checkXMas to allow
        /// cancelling of the function.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool Christmas()
        {
            if (Hooks.Game.Christmas != null)
                return Hooks.Game.Christmas() == HookResult.Continue;
            return true;
        }
    }
}
