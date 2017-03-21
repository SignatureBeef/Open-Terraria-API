using System;
using System.Net;
using System.Net.Sockets;
using Terraria;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	public class AsyncSocket : Terraria.Net.Sockets.ISocket
	{
		private SocketServer _server;
		private RemoteAddress _remoteAddress;

		private Socket _socket;

		private TcpClient _connection;

		private static AsyncArgsPool<ReceiveEventArgs> _receivePool = new AsyncArgsPool<ReceiveEventArgs>();
		private static AsyncArgsPool<SendEventArgs> _sendPool = new AsyncArgsPool<SendEventArgs>();

		public AsyncSocket()
		{
		}

		public void OnReceiveComplete(ReceiveEventArgs args)
		{
			if (args.SocketError != System.Net.Sockets.SocketError.Success || args.BytesTransferred == 0)
			{
				//Release back to the pool
				args.Socket = null;
				_receivePool.PushBack(args);

				Close();
			}
			else
			{
				// give to terraria
				args.ReceiveCallback(args.UserToken, args.BytesTransferred);

				args.Socket = null;
				_receivePool.PushBack(args);
			}
		}

		public void OnSendComplete(SendEventArgs args)
		{
			if (args.SocketError != System.Net.Sockets.SocketError.Success)
			{
				//Release back to the pool
				args.Socket = null;
				_sendPool.PushBack(args);

				Close();
			}
			else
			{
				// give to terraria
				args.SendCallback(args.UserToken);

				args.Socket = null;
				_sendPool.PushBack(args);
			}
		}

		public AsyncSocket(Socket socket)
		{
			this._socket = socket;

			var endpoint = (IPEndPoint)socket.RemoteEndPoint;
			this._remoteAddress = new TcpAddress(endpoint.Address, endpoint.Port);
		}

		public bool IsConnected()
		{
			return
				(_socket != null && _socket.Connected)
				|| (_connection != null && _connection.Connected)
			;
		}

		public bool IsDataAvailable()
		{
			if (_socket != null && _socket.Connected)
				return _socket.Available > 0;
			else if (_connection != null && _connection.Connected)
				return _connection.Available > 0;

			return false;
		}

		public RemoteAddress GetRemoteAddress()
		{
			return _remoteAddress;
		}

		public bool StartListening(SocketConnectionAccepted callback)
		{
			var any = IPAddress.Any;
			string ipString;
			if (Program.LaunchParameters.TryGetValue("-ip", out ipString) && !IPAddress.TryParse(ipString, out any))
			{
				any = IPAddress.Any;
			}

			_server = new SocketServer(any, Netplay.ListenPort);
			_server.SetConnectionAcceptedCallback((socket) =>
			{
				socket.NoDelay = true;

				var imp = new AsyncSocket(socket);
				Console.WriteLine(imp.GetRemoteAddress() + " is connecting...");
				callback(imp);
			});
			return _server.Start();
		}

		public void StopListening()
		{
			_server.Stop();
		}

		public void Close()
		{
			if (_server != null)
			{
				_server.Stop();
			}

			if (_socket != null)
			{
				try
				{
					_socket.Close();
				}
				catch (SocketException) { }
				catch (ObjectDisposedException) { }
			}

			if (_connection != null)
			{
				try
				{
					_connection.Close();
				}
				catch (SocketException) { }
				catch (ObjectDisposedException) { }
			}
		}

		public void Connect(RemoteAddress address)
		{
			if (this._connection == null)
			{
				this._connection = new TcpClient();
				this._connection.NoDelay = true;
			}

			TcpAddress tcpAddress = (TcpAddress)address;
			this._connection.Connect(tcpAddress.Address, tcpAddress.Port);
			this._remoteAddress = address;
		}

		public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null)
		{
			var arg = _receivePool.PopFront();
			arg.SetBuffer(data, offset, size);
			arg.ReceiveCallback = callback;
			arg.UserToken = state;
			arg.Socket = this;

			if (_socket != null)
			{
				if (!_socket.ReceiveAsync(arg))
				{
					//The receive was processed synchronously which means the callback wont be executed.
					OnReceiveComplete(arg);
				}
			}
			else if (_connection != null)
			{
				if (!_connection.Client.ReceiveAsync(arg))
				{
					//The receive was processed synchronously which means the callback wont be executed.
					OnReceiveComplete(arg);
				}
			}
		}

		public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null)
		{
			var arg = _sendPool.PopFront();
			arg.SetBuffer(data, offset, size);
			arg.SendCallback = callback;
			arg.UserToken = state;
			arg.Socket = this;

			if (_socket != null)
			{
				if (!_socket.SendAsync(arg))
				{
					//The receive was processed synchronously which means the callback wont be executed.
					OnSendComplete(arg);
				}
			}
			else if (_connection != null)
			{
				if (!_connection.Client.SendAsync(arg))
				{
					//The receive was processed synchronously which means the callback wont be executed.
					OnSendComplete(arg);
				}
			}
		}

		public void SendQueuedPackets()
		{

		}
	}
}
