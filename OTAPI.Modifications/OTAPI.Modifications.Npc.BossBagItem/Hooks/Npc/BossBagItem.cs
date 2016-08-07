namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate HookResult BossBagItemHandler
            (
                global::Terraria.NPC npc,
                ref int X,
                ref int Y,
                ref int Width,
                ref int Height,
                ref int Type,
                ref int Stack,
                ref bool noBroadcast,
                ref int pfix,
                ref bool noGrabDelay,
                ref bool reverseLookup
            );
            #endregion

            /// <summary>
            /// Occurs when a boss item is to drop
            /// Arg 1:  The npc instance
            /// TODO: the following arguments are vanilla, these need to be documented.
            /// </summary>
            /// <remarks>Currently only supported using the server API</remarks>
            public static BossBagItemHandler BossBagItem;
        }
    }
}
