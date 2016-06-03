namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// This method is injected into the start of the startDedInput method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool startDedInput()
        {
            if (Hooks.Command.StartCommandThread != null)
                return Hooks.Command.StartCommandThread() == HookResult.Continue;
            return true;
        }
    }
}
