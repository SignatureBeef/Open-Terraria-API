using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	/// <summary>
	/// Defines a completion callback so that the application can know when the operation has been completed
	/// This is based around terrarias ISocket callback code.
	/// </summary>
	public struct SendArgCallback
	{
		public SocketSendCallback callback;
		public object state;
	}

	/// <summary>
	/// Defines a request for data to be sent to a client
	/// </summary>
	public struct SendRequest
	{
		public ArraySegment<byte> segment;
		public SocketSendCallback callback;
		public object state;
	}

	public class SendArgs : AsyncSocketEventArgs
	{
		private volatile Queue<SendArgCallback> callbacks = new Queue<SendArgCallback>();
		private volatile List<ArraySegment<byte>> segments = new List<ArraySegment<byte>>();

		/// <summary>
		/// Called when the args have been successfully sent
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCompleted(SocketAsyncEventArgs e)
		{
			conn?.SendCompleted(this);
		}

		/// <summary>
		/// Enqueues a new packet into the packets to be send in this operation
		/// </summary>
		/// <param name="request"></param>
		public void Enqueue(SendRequest request)
		{
			callbacks.Enqueue(new SendArgCallback()
			{
				callback = request.callback,
				state = request.state
			});

			segments.Add(request.segment);
		}

		/// <summary>
		/// Prepares the arg for sending
		/// </summary>
		public bool Prepare()
		{
			if (segments.Count > 0)
			{
				// the base will pin these segments to native code when we do this
				// this means we cannot alter the list after we do this
				base.BufferList = segments;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Notifies all registered callbacks when all queued data has been sent in this operation
		/// </summary>
		internal void NotifySent()
		{
			while (callbacks.Count > 0)
			{
				var cb = callbacks.Dequeue();
				cb.callback(cb.state);
			}

			base.BufferList.Clear();
			base.BufferList = null;
		}
	}
}
