namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            #region Handlers
            public delegate HookHandler ReceiveDataHandler
            (
                global::Terraria.MessageBuffer buffer,
                ref byte packetId,
                ref int readOffset,
                ref int start,
                ref int length,
                ref int messageType
            );
            #endregion

            /// <summary>
            /// Occurs at the start of the GetData method.
            /// Arg 1: buffer
            //      2: packetId,
            //      3: readOffset,
            //      4: start,
            //      5: length,
            //      6: messageType
            /// </summary>
            public static ReceiveDataHandler ReceiveData;

        }
    }
}
