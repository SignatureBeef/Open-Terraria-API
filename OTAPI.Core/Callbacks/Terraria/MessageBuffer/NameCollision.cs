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
            var res = Hooks.Net.Player.NameCollision?.Invoke(player);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }
    }
}
