using System.Net;
using System.Net.Sockets;
using Terraria.Net;

namespace OTAPI.Sockets
{
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

            StartReceiving();
        }

        public RemoteAddress GetRemoteAddress() => _remoteAddress;

        public bool IsConnected() => _connected;

        public bool IsDataAvailable() => available > 0; // storeWriteOffset > 0;
    }
}
