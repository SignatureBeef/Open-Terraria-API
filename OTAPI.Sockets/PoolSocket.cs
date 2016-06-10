using System;
using System.Net;
using System.Net.Sockets;
using Terraria;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
    public class ReceiveEventArgs : PoolSocketEventArgs
    {

    }
    public class SendEventArgs : PoolSocketEventArgs
    {

    }


    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {
        private Socket _socket;
        private RemoteAddress _remoteAddress;
        private bool _connected;

        public PoolSocket(Socket socket)
        {
            this._socket = socket;

            this._socket.NoDelay = true;
            var endpoint = (IPEndPoint)this._socket.RemoteEndPoint;
            this._remoteAddress = new TcpAddress(endpoint.Address, endpoint.Port);

            _connected = true;
        }

        public RemoteAddress GetRemoteAddress() => _remoteAddress;

        public bool IsConnected() => _connected;

        public bool IsDataAvailable() => false;//TODO
    }


    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {
        private bool _disconnect = false;
        private TcpListener _listener;

        public PoolSocket()
        {

        }

        public void Connect(RemoteAddress address)
        {
        }

        public void Close()
        {

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
                Console.WriteLine($"ServerLoop terminated with exception\n{ex}");
            }
        }

        private void SinkThread()
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

                                    socket.Flush();
                                }
                            }
                        }
                    }

                    //No clients, we can decrease the interval
                    if (!any)
                        System.Threading.Thread.Sleep(200);
                    else System.Threading.Thread.Sleep(16);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ServerLoop terminated with exception\n{ex}");
            }
        }
    }

    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {

        public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null)
        {
        }

        public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null)
        {
        }

        public void Flush()
        {

        }
    }



    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {
        private static ArgsPool<ReceiveEventArgs> _receivePool = new ArgsPool<ReceiveEventArgs>();
        private static ArgsPool<ReceiveEventArgs> _sendPool = new ArgsPool<ReceiveEventArgs>();
    }
}
