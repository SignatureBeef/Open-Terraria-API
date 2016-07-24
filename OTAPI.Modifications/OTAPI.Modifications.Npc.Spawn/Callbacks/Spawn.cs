namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        /// <summary>
        /// Called from Terraria.NPC.NewNPC after the NPC has been created
        /// and is about to be spawned into the world.
        /// </summary>
        /// <param name="index">Index of the npc in Terraria.Main.npc</param>
        /// <returns>True to continue vanilla code, false to cancel it.</returns>
        /// <remarks>Ideally the consumer will set the index to 200 to fully cancel the event</remarks>
        internal static bool Spawn(ref int index)
        {
            if (Hooks.Npc.Spawn?.Invoke(ref index) == HookResult.Cancel)
                return false;

            return true;
        }
    }
}
