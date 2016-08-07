namespace OTAPI.Callbacks.Terraria
{
    internal static partial class Npc
    {
        /// <summary>
        /// This method injected into the DropBossBags call, replacing the server NewItem calls.
        /// We check the result for -1 to cancel the function.
        /// </summary>
        internal static int BossBagItem
        (
            global::Terraria.NPC npc,
            int X,
            int Y,
            int Width,
            int Height,
            int Type,
            int Stack,
            bool noBroadcast,
            int pfix,
            bool noGrabDelay,
            bool reverseLookup
        )
        {
            //Allow altering of our local variables so the item can be changed etc
            if (Hooks.Npc.BossBagItem?.Invoke
            (
                npc,
                ref X,
                ref Y,
                ref Width,
                ref Height,
                ref Type,
                ref Stack,
                ref noBroadcast,
                ref pfix,
                ref noGrabDelay,
                ref reverseLookup
            ) == HookResult.Cancel) return -1; //This will be checked for in the patcher using a custom if block.

            return global::Terraria.Item.NewItem(X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
        }
    }
}
