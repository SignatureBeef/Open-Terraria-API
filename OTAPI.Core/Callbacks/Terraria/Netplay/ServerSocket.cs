namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Netplay
    {
        /// <summary>
        /// This method replaces the "new TcpSocket" call in server loop, allowing
        /// custom server socket implementations.
        /// </summary>
        internal static global::Terraria.Net.Sockets.ISocket ServerSocketCreate()
        {
            global::Terraria.Net.Sockets.ISocket socket = null;

            if (Hooks.Net.Socket.ServerSocketCreate != null)
                socket = Hooks.Net.Socket.ServerSocketCreate();

            //A value is always required
            if (socket == null)
                return new global::Terraria.Net.Sockets.TcpSocket();

            return socket;
        }
    }
}
