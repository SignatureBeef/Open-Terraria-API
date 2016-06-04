namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class NetMessage
    {
        /// <summary>
        /// This method is called immediately before any code of the greetPlayer method executes.
        /// </summary>
        internal static bool GreetPlayerBegin(ref int playerId)
        {
            if (Hooks.Net.PreGreetPlayer != null)
                return Hooks.Net.PreGreetPlayer(ref playerId) == HookResult.Continue;
            return true;
        }


        /// <summary>
        /// This method is called after the vanilla greetPlayer method finishes.
        /// </summary>
        /// <param name="playerId"></param>
        internal static void GreetPlayerEnd(int playerId)
        {
            if (Hooks.Net.PostGreetPlayer != null)
                Hooks.Net.PostGreetPlayer(playerId);
        }
    }
}
