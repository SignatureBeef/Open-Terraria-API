namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            public static partial class Socket
            {
                #region Handlers
                public delegate global::Terraria.Net.Sockets.ISocket ServerSocketCreateHandler();
                #endregion

                /// <summary>
                /// Occurs when the server socket is to be created.
                /// Return null if you wish for default functionality.
                /// </summary>
                public static ServerSocketCreateHandler Create;
            }
        }
    }
}
