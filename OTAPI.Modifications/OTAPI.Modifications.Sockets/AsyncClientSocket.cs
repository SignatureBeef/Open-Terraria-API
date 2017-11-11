using System;
using System.Collections.Concurrent;
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
		public bool IsDataAvailable => recvBytes > 0;

		public int MaxPacketsPerOperation { get; set; } = 100;

		/// <summary>
		/// Used to know if there is a send operation in progress to the client
		/// </summary>
		protected volatile bool sending = false;

		/// <summary>
		/// Store packets while an operation is in progress
		/// </summary>
		protected ConcurrentQueue<SendRequest> _sendQueue = new ConcurrentQueue<SendRequest>();

		protected int recvBytes;

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

		/// <summary>
		/// Despatches received data from the arg through to terrarias internal buffers
		/// </summary>
		protected virtual void DespatchData(ReceiveArgs args)
		{
			var id = this.RemoteClient.Id;
			MessageBuffer obj = NetMessage.buffer[id];
			lock (obj)
			{
				if (!this.RemoteClient.IsActive)
				{
					this.RemoteClient.IsActive = true;
					this.RemoteClient.State = 0;
				}

				Buffer.BlockCopy(args.Buffer, args.Offset, NetMessage.buffer[id].readBuffer, NetMessage.buffer[id].totalData, recvBytes);
				NetMessage.buffer[id].totalData += recvBytes;
				NetMessage.buffer[id].checkBytes = true;

				recvBytes = 0;
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
					var receiving = false;
					while (!receiving)
					{
						recvBytes += args.BytesTransferred;

						this.DespatchData(args);

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
			outgoing.NotifySent();

			sending = SendMore(outgoing);

			if (!sending)
			{
				outgoing.conn = null;
				Server.SendSocketPool.PushBack(outgoing);
			}
		}

		public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null)
		{
			// we don't use terrarias async callbacks, we do it ourself so we arent limited to their buffer sizes
		}

		public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null)
		{
			_sendQueue.Enqueue(new SendRequest()
			{
				segment = new ArraySegment<byte>(data, offset, size),
				callback = callback,
				state = state
			});

			if (!sending)
			{
				sending = SendMore();
			}
		}

		/// <summary>
		/// Attempts to send as many queued packets in one operation
		/// </summary>
		/// <param name="preallocated">A previously allocated arg</param>
		/// <returns>True when the argument is queued to send</returns>
		private bool SendMore(SendArgs preallocated = null)
		{
			bool queued = false;

			if (preallocated == null)
			{
				preallocated = Server.SendSocketPool.PopFront();
				preallocated.SetBuffer(null, 0, 0);
			}

			preallocated.conn = this;
			
			while (_sendQueue.TryDequeue(out SendRequest request))
			{
				preallocated.Enqueue(request);
			}

			preallocated.Prepare();

			try
			{
				queued = Source.SendAsync(preallocated);
			}
			catch (SocketException e)
			{
				HandleError(e.SocketErrorCode);
			}
			catch (ObjectDisposedException)
			{
				HandleError(SocketError.OperationAborted);
			}

			return queued;
		}
	}
}
