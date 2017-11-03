using System;
using System.Net.Sockets;
using System.Threading;
using Terraria;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	public class AsyncClientSocket
	{
		public Socket Source { get; private set; }
		public AsyncServerSocket Server { get; private set; }
		public RemoteClient RemoteClient { get; private set; }

		protected byte[] recvSrcBuffer;

		protected volatile int error = 0;

		public event ConnectionDisconnectHandler OnDisconnect;

		public string IpAddress { get; private set; }

		public bool IsActive { get; private set; }
		public bool IsDataAvailable => (recvBytes - recvBytesConsumed) > 0;

		protected int recvBytes;
		protected int recvBytesConsumed;

		public AsyncClientSocket(AsyncServerSocket server, Socket source)
		{
			this.Server = server;
			this.Source = source;

			source.LingerState = new LingerOption(true, 10);
		}

		public void StartReading()
		{
			IpAddress = Source.RemoteEndPoint.ToString();

			recvSrcBuffer = new byte[4096];

			BeginReadFromSource();

			IsActive = true;
		}

		void BeginReadFromSource()
		{
			var incoming = Server.ReceiveSocketPool.PopFront();
			incoming.SetBuffer(recvSrcBuffer, 0, recvSrcBuffer.Length);
			incoming.origin = Source;
			incoming.conn = this;

			if (!Source.ReceiveAsync(incoming))
				ReceiveCompleted(incoming);
		}

		void HandleError(SocketError err)
		{
			if (Interlocked.CompareExchange(ref this.error, (int)err, (int)SocketError.Success) != (int)SocketError.Success)
				return;

			this.Close();
		}

		public void SetRemoteClient(RemoteClient remoteClient)
		{
			this.RemoteClient = remoteClient;
		}

		public void Close()
		{
			if (IsActive)
			{
				IsActive = false;
				try
				{
					Source.Close();
				}
				catch (SocketException) { }
				catch (ObjectDisposedException) { }

				OnDisconnect?.Invoke(this);
			}
		}

		public virtual void ReceiveCompleted(ReceiveArgs args)
		{
			try
			{
				bool release = false;

				if (args.SocketError != SocketError.Success)
				{
					release = true;
					HandleError(args.SocketError);
				}
				else if (args.BytesTransferred == 0)
				{
					release = true;
					HandleError(SocketError.Disconnecting);
				}
				else
				{
					var bytes = args.BytesTransferred;
					var receiving = false;
					while (!receiving)
					{
						recvBytes += bytes;

						var id = this.RemoteClient.Id;
						MessageBuffer obj = NetMessage.buffer[id];
						lock (obj)
						{
							if (!this.RemoteClient.IsActive)
							{
								this.RemoteClient.IsActive = true;
								this.RemoteClient.State = 0;
							}

							var len = recvBytes;
							Buffer.BlockCopy(args.Buffer, 0, NetMessage.buffer[id].readBuffer, NetMessage.buffer[id].totalData, len);
							NetMessage.buffer[id].totalData += len;
							NetMessage.buffer[id].checkBytes = true;

							recvBytes = 0;
							recvBytesConsumed = 0;
						}

						var left = args.Buffer.Length - recvBytes;

						if (left <= 0)
						{
							return;
						}

						args.SetBuffer(args.Buffer, recvBytes, left);
						try
						{
							receiving = args.origin.ReceiveAsync(args);
						}
						catch (ObjectDisposedException)
						{
							receiving = false;
						}

						if (receiving) bytes = args.BytesTransferred;
					}

					if (!receiving) release = true;
				}

				if (release)
				{
					args.conn = null;
					args.origin = null;
					Server.ReceiveSocketPool.PushBack(args);
				}

				this.RemoteClient.IsReading = false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public virtual void SendCompleted(SendArgs outgoing)
		{
			outgoing.callback(outgoing.state);
			outgoing.state = null;
			outgoing.callback = null;
			outgoing.conn = null;
			Server.SendSocketPool.PushBack(outgoing);
		}

		public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null)
		{
			// we don't use terrarias async callbacks, we do it ourself so we arent limited to their buffer sizes
		}

		public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null)
		{
			var outgoing = Server.SendSocketPool.PopFront();
			outgoing.SetBuffer(data, offset, size);
			outgoing.conn = this;
			outgoing.callback = callback;
			outgoing.state = state;

			try
			{
				if (!Source.SendAsync(outgoing))
					SendCompleted(outgoing);
			}
			catch (ObjectDisposedException) { }
		}
	}
}
