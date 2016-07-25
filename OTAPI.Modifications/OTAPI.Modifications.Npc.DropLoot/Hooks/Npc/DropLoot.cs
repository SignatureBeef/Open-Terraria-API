namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate HookResult PreDropLootHandler
            (
                ref int itemId,
                ref int x,
                ref int y,
                ref int width,
                ref int height,
                ref int type,
                ref int stack,
                ref bool noBroadcast,
                ref int prefix,
                ref bool noGrabDelay,
                ref bool reverseLookup
            );
            public delegate void PostDropLootHandler
            (
                int x,
                int y,
                int width,
                int height,
                int type,
                int stack,
                bool noBroadcast,
                int prefix,
                bool noGrabDelay,
                bool reverseLookup
            );
            #endregion

            /// <summary>
            /// Occurs at the start of the NetDefaults(int) method.
            /// Arg 1:  Item id
            ///     2:  Type
            //      3:  x
            //      4:  y
            //      5:  width
            //      6:  height
            //      7:  type
            //      8:  stack
            //      9:  noBroadcast
            //      10: prefix
            //      11: noGrabDelay
            //      12: reverseLookup
            /// </summary>
            public static PreDropLootHandler PreDropLoot;

            /// <summary>
            /// Occurs when the NetDefaults(int) method ends
            /// Arg 1:  Type
            //      2:  x
            //      3:  y
            //      4:  width
            //      5:  height
            //      6:  type
            //      7:  stack
            //      8:  noBroadcast
            //      9: prefix
            //      10: noGrabDelay
            //      11: reverseLookup
            /// </summary>
            public static PostDropLootHandler PostDropLoot;
        }
    }
}
