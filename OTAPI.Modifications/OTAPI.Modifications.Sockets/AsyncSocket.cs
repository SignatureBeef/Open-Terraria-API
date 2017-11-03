using System;
using System.Net;
using System.Net.Sockets;
using Terraria;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	/// <summary>
	/// A socket that is used by terraria as a server and client socket which uses <see cref="SocketAsyncEventArgs"/>
	/// </summary>
	public class AsyncSocket : Terraria.Net.Sockets.ISocket
	{
		private AsyncServerSocket _server;
		private AsyncClientSocket _client;
		private RemoteAddress _remoteAddress;

		/// <summary>
		/// Creates a new server socket
		/// </summary>
		public AsyncSocket() { }

		/// <summary>
		/// Creates a new client socket
		/// </summary>
		/// <param name="server">The server which is creating the socket</param>
		/// <param name="socket"></param>
		public AsyncSocket(AsyncServerSocket server, Socket socket)
		{
			//client
			_client = new AsyncClientSocket(server, socket);

			var endpoint = (IPEndPoint)socket.RemoteEndPoint;
			this._remoteAddress = new TcpAddress(endpoint.Address, endpoint.Port);
		}

		public void SetRemoteClient(RemoteClient remoteClient)
		{
			_client.SetRemoteClient(remoteClient);
		}

		public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null)
		{
			_client?.AsyncReceive(data, offset, size, callback, state);
		}

		public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null)
		{
			_client?.AsyncSend(data, offset, size, callback, state);
		}

		public void Close()
		{
			_client?.Close();
		}

		public void Connect(RemoteAddress address)
		{
			throw new NotImplementedException();
		}

		public RemoteAddress GetRemoteAddress() => _remoteAddress;

		public bool IsConnected() => _client?.IsActive == true;

		public bool IsDataAvailable() => _client?.IsDataAvailable == true;

		public void SendQueuedPackets()
		{

		}

		public bool StartListening(SocketConnectionAccepted callback)
		{
			if (_server == null)
			{
				_server = new AsyncServerSocket(callback);
			}
			return _server.Listen();
		}

		public void StopListening()
		{
			_server.Stop();
		}

		public void StartReading()
		{
			_client?.StartReading();
		}
	}
}
