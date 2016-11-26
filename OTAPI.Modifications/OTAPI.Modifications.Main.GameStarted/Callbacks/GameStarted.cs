namespace OTAPI.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// This method is injected right before the server starts the update loop
        /// </summary>
        internal static void GameStarted() => Hooks.Game.Started?.Invoke();
    }
}
