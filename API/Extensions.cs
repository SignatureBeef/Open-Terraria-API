using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;

namespace OTA
{
    /// <summary>
    /// Socket extensions.
    /// </summary>
    public static class SocketExtensions
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
    }

    /// <summary>
    /// Linq extensions.
    /// </summary>
    public static class LinqExtensions
    {
        static readonly Random _rand = new Random();

        /// <summary>
        /// Selects a random item from the list
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            var list = enumerable as IList<T> ?? enumerable.ToList();
            var count = list.Count;
            if (count == 0)
                return default(T);
            return list.ElementAt(_rand.Next(0, count));
        }

        /// <summary>
        /// Shuffle the specified data.
        /// </summary>
        /// <remarks>Based on https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle</remarks>
        /// <param name="data">Data.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> Shuffle<T>(this T[] data)
        {
            var n = data.Length;  
            while (n > 1)
            {  
                n--;  
                var j = _rand.Next(n + 1);
                T value = data[j];  
                data[j] = data[n];  
                data[n] = value;  
            }  

            return data;
        }
    }

    /// <summary>
    /// Assembly extensions.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets types where they can be successfully loaded. This is useful when asseblies use lazy loading and don't have an optional component.
        /// </summary>
        /// <returns>The types loaded.</returns>
        /// <param name="assembly">Assembly.</param>
        public static Type[] GetTypesLoaded(this System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).ToArray();
            } 
        }
    }
}

