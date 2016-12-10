namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            #region Handlers
            public delegate HookResult ReceiveDataHandler
            (
                global::Terraria.MessageBuffer buffer,
                ref byte packetId,
                ref int readOffset,
                ref int start,
                ref int length
            );
            #endregion

            /// <summary>
            /// Occurs at the start of the GetData method.
            /// Arg 1: buffer
            //      2: packetId,
            //      3: readOffset,
            //      4: start,
            //      5: length
            /// </summary>
            public static ReceiveDataHandler ReceiveData;
        }
    }
}
