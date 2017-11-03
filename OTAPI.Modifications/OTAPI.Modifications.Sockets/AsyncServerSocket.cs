using System;
using System.Linq;
using System.Net;
using Terraria;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	public class AsyncServerSocket
	{
		private SocketServer _server;

		public AsyncArgsPool<ReceiveArgs> ReceiveSocketPool = new AsyncArgsPool<ReceiveArgs>(nameof(ReceiveArgs));
		public AsyncArgsPool<SendArgs> SendSocketPool = new AsyncArgsPool<SendArgs>(nameof(SendArgs));

		private SocketConnectionAccepted _callback;

		public AsyncServerSocket(SocketConnectionAccepted callback)
		{
			_callback = callback;
		}

		public void Stop()
		{
			_server.Stop();
			_server = null;
		}

		public bool Listen()
		{
			if (_server == null)
			{
				var any = IPAddress.Any;
				string ipString;
				if (Terraria.Program.LaunchParameters.TryGetValue("-ip", out ipString) && !IPAddress.TryParse(ipString, out any))
				{
					any = IPAddress.Any;
				}

				_server = new SocketServer(any, Netplay.ListenPort);
				_server.SetConnectionAcceptedCallback((socket) =>
				{
					try
					{
						var imp = new AsyncSocket(this, socket);
						//Console.WriteLine(imp.GetRemoteAddress() + " is connecting...");
						_callback(imp);

						var remoteClient = Netplay.Clients.SingleOrDefault(x => x != null && x.Socket == imp);
						if (remoteClient != null)
						{
							imp.SetRemoteClient(remoteClient);
							imp.StartReading();
						}
						else
						{
							socket.Close();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				});
				return _server.Start();
			}
			return false;
		}
	}
}
