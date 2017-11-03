using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OTAPI.Sockets
{
	public class SocketServer
	{
		private IPAddress _ipaddress;
		private int _port;

		private TcpListener _listener;
		private volatile bool _disconnect;

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

			new Thread(ListenThread).Start();

			return true;
		}

		public void Stop()
		{
			_disconnect = true;

			_listener.Stop();
		}

		private readonly object _sync = new object();

		private void ListenThread(object state)
		{
			try
			{
				var reset = new AutoResetEvent(false);
				while (!_disconnect)
				{
					_listener.AcceptSocketAsync()
						.ContinueWith(async (task) =>
						{
							reset.Set();
							if (task != null && task.IsCompleted)
							{
								if (task.Result != null && task.Result.Connected)
								{
									var socket = task.Result;

									await Task.Run(() =>
									{
										try
										{
											lock (_sync) // prevent slots being misallocated
											{
												if (!_disconnect)
												{
													this._socketAccepted(socket);
												}
												else
												{
													socket.Close();
												}
											}
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex);
										}
									});
								}
							}
						});
					reset.WaitOne();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{nameof(ListenThread)} terminated with exception\n{ex}");
			}

			Terraria.Netplay.IsListening = false;
			Console.WriteLine($"{nameof(ListenThread)} has exited");
		}
	}
}
