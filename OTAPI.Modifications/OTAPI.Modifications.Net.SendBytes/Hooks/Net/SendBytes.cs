namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            #region Handlers
            public delegate HookResult SendBytesHandler
            (
                ref int remoteClient,
                ref byte[] data,
                ref int offset,
                ref int size,
                ref global::Terraria.Net.Sockets.SocketSendCallback callback,
                ref object state
            );
            #endregion

            /// <summary>
            /// Occurs when the server is about to send data to a client
            /// </summary>
            public static SendBytesHandler SendBytes;
        }
    }
}
