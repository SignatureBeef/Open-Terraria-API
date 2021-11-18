namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            #region Handlers
            public delegate HookResult SendDataHandler
            (
                ref int bufferId,
                ref int msgType,
                ref int remoteClient,
                ref int ignoreClient,
                ref Terraria.Localization.NetworkText text,
                ref int number,
                ref float number2,
                ref float number3,
                ref float number4,
                ref int number5,
                ref int number6,
                ref int number7,
                ref float number8
            );
            #endregion

            /// <summary>
            /// Occurs at the start of the SendData method.
            /// Arg 1: bufferId,
            //      2: msgType,
            //      3: remoteClient,
            //      4: ignoreClient,
            //      5: text,
            //      6: number,
            //      7: number2,
            //      8: number3,
            //      9: number4,
            //      10: number5,
            //      11: number6,
            //      12: number7
            /// </summary>
            public static SendDataHandler SendData;
        }
    }
}
