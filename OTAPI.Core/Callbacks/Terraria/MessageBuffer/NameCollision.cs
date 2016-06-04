namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class MessageBuffer
    {
        /// <summary>
        /// This method is injected on the if block that surrounds the player name
        /// collision kick.
        /// </summary>
        internal static bool NameCollision(global::Terraria.Player player)
        {
            if (Hooks.Net.NameCollision != null)
                return Hooks.Net.NameCollision(player) == HookResult.Continue;
            return true;
        }
    }
}
