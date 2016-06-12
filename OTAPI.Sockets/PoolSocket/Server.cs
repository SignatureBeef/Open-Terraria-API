using System;
using System.Net;
using System.Net.Sockets;
using Terraria;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
    //Server implementation
    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {
        private bool _disconnect = false;
        private TcpListener _listener;

        public PoolSocket()
        {

        }

        public void Close()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Close();
                }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }
            }
            if (_listener != null)
            {
                _listener.Stop();
            }
            System.Diagnostics.Debug.WriteLine("Closed socket");
        }

        public bool StartListening(SocketConnectionAccepted callback)
        {
            _disconnect = false;

            var any = IPAddress.Any;
            string ipString;
            if (Program.LaunchParameters.TryGetValue("-ip", out ipString) && !IPAddress.TryParse(ipString, out any))
            {
                any = IPAddress.Any;
            }

            _listener = new TcpListener(any, Netplay.ListenPort);

            try
            {
                this._listener.Start();
            }
            catch (Exception)
            {
                return false;
            }

            (new System.Threading.Thread(ListenThread)).Start(callback);
            (new System.Threading.Thread(DrainThread)).Start();

            return true;
        }

        public void StopListening()
        {
            _disconnect = true;
        }

        private void ListenThread(object state)
        {
            var callback = (SocketConnectionAccepted)state;

            try
            {
                while (!_disconnect)
                {
                    _listener.Server.Poll(500000, SelectMode.SelectRead);

                    if (_disconnect) break;

                    // Accept new clients
                    while (_listener.Pending())
                    {
                        var socket = _listener.AcceptSocket();
                        var imp = new PoolSocket(socket);
                        Console.WriteLine(imp.GetRemoteAddress() + " is connecting...");
                        callback(imp);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(ListenThread)} terminated with exception\n{ex}");
            }
        }

        /// <summary>
        /// The purpose of this thread is to look for each instance of PoolSocket
        /// and to flush data to be sent to the client.
        /// This method might change as i'm testing it not being on the server thread 
        /// </summary>
        private void DrainThread()
        {
            try
            {
                while (!_disconnect)
                {
                    bool any = false;
                    for (var x = 0; x < Netplay.Clients.Length; x++)
                    {
                        var client = Netplay.Clients[x];
                        if (client != null)
                        {
                            var socket = client.Socket as PoolSocket;
                            if (socket != null)
                            {
                                if (socket.IsConnected())
                                {
                                    any = true;

                                    socket.TrySend();
                                }
                            }
                        }
                    }

                    ////DEBUG
                    ////Long delay to test that messages are in order
                    //System.Threading.Thread.Sleep(2000);

                    //No clients, we can decrease the interval
                    if (!any)
                        System.Threading.Thread.Sleep(200);
                    else System.Threading.Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(DrainThread)} terminated with exception\n{ex}");
            }
        }
    }
}
