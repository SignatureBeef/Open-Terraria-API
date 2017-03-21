using System;
using System.Net;
using System.Net.Sockets;

namespace OTAPI.Sockets
{
	delegate void SocketAccepted(Socket socket);

	class SocketServer
	{
		private IPAddress _ipaddress;
		private int _port;

		private TcpListener _listener;
		private bool _disconnect;

		private SocketAccepted _socketAccepted;

		public SocketServer(IPAddress ipaddress, int port)
		{
			this._ipaddress = ipaddress;
			this._port = port;
		}

		public void SetConnectionAcceptedCallback(SocketAccepted callback)
		{
			this._socketAccepted = callback;
		}

		public bool Start()
		{
			if (this._socketAccepted == null)
			{
				throw new InvalidOperationException($"Please specify the accepted callback using {nameof(SetConnectionAcceptedCallback)} before starting the server");
			}

			_listener = new TcpListener(_ipaddress, _port);

			try
			{
				this._listener.Start();
			}
			catch (Exception)
			{
				Console.WriteLine($"Failed to start server on ip:port {_ipaddress}:{_port}");
				return false;
			}

			new System.Threading.Thread(ListenThread).Start();

			return true;
		}

		public void Stop()
		{
			_disconnect = true;

			_listener.Stop();
		}

		private void ListenThread(object state)
		{
			try
			{
				while (!_disconnect)
				{
					_listener.Server.Poll(500000, SelectMode.SelectRead);

					if (_disconnect) break;

					// Accept new clients
					while (_listener.Pending())
					{
						this._socketAccepted(_listener.AcceptSocket());
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{nameof(ListenThread)} terminated with exception\n{ex}");
			}
		}
	}
}
