using System;
using System.Net.Sockets;
using Terraria;

namespace OTA.Extensions
{
    /// <summary>
    /// Remote client/socket extensions
    /// </summary>
    public static class RemoteExtensions
    {
        #if Full_API
        /// <summary>
        /// Determines if the client is in the playing state
        /// </summary>
        /// <returns><c>true</c> if is playing the specified sock; otherwise, <c>false</c>.</returns>
        /// <param name="sock">Sock.</param>
        public static bool IsPlaying(this Terraria.RemoteClient sock)
        {
            return sock.State == (int)OTA.Sockets.SlotState.PLAYING;
        }

        /// <summary>
        /// Determines if the client is in a state where they can be sent water
        /// </summary>
        /// <returns><c>true</c> if can send water the specified sock; otherwise, <c>false</c>.</returns>
        /// <param name="sock">Sock.</param>
        public static bool CanSendWater(this Terraria.RemoteClient sock)
        {
            //return state >= 3;
            return (Terraria.NetMessage.buffer[sock.Id].broadcast || sock.State >= (int)OTA.Sockets.SlotState.SENDING_TILES) && sock.Socket.IsConnected();
        }

        /// <summary>
        /// Gets the clients remote address
        /// </summary>
        /// <returns>The address.</returns>
        /// <param name="sock">Sock.</param>
        public static string RemoteAddress(this Terraria.RemoteClient sock)
        {
            return sock.Socket.GetRemoteAddress().ToString();
        }

        /// <summary>
        /// Gets the clients remote IP address
        /// </summary>
        /// <returns>The IP address.</returns>
        /// <param name="sock">Sock.</param>
        public static string RemoteIPAddress(this Terraria.RemoteClient sock)
        {
            var addr = sock.Socket.GetRemoteAddress().ToString();
            var ix = addr.IndexOf(':');
            if (ix > 0) addr = addr.Substring(0, ix);

            return addr;
        }

        /// <summary>
        /// Kicks the connection
        /// </summary>
        /// <param name="sock">Sock.</param>
        /// <param name="reason">Reason.</param>
        public static void Kick(this Terraria.RemoteClient sock, string reason)
        {
            Terraria.NetMessage.SendData((int)Packet.DISCONNECT, sock.Id, -1, reason);
        }
        #endif

        /// <summary>
        /// Sagely closes the socket
        /// </summary>
        /// <param name="socket">Socket.</param>
        public static void SafeClose(this Socket socket)
        {
            if (socket == null)
                return;

            try
            {
                socket.Close();
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Safely shuts the socket down
        /// </summary>
        /// <param name="socket">Socket.</param>
        public static void SafeShutdown(this Socket socket)
        {
            if (socket == null)
                return;

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Determines if the client is using an OTA client mod.
        /// </summary>
        /// <returns><c>true</c> if is OTA client the specified client; otherwise, <c>false</c>.</returns>
        /// <param name="client">Client.</param>
        public static bool IsOTAClient(this Terraria.RemoteClient client)
        {
            return client.Socket != null && (client.Socket as OTA.Sockets.ClientConnection).IsOTAClient;
        }
    }
}

