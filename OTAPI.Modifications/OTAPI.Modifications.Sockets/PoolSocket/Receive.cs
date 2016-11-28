using System;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
	{
		/// <summary>
		/// 
		/// </summary>
		private byte[] receiveBuffer;
		//private byte[] storeBuffer;
		private int storeWriteOffset = 0;
		private int storeReadOffset = 0;
		private readonly object _rxSyncRoot = new object();

		protected void StartReceiving()
		{
			if (receiveBuffer != null)
				throw new InvalidOperationException("Receive buffer already initialised");

			receiveBuffer = new byte[1024 * 1024 * 4]; //Receives packets
													   //Stores the data, ready for the AsyncReceive method to request data
													   //The store is allocated a much larger buffer as locally, we can fill
													   //it quicker than what Terraria reads it.
													   //storeBuffer = new byte[1024 * 4];

			ReceiveData();
		}

		private void ReceiveData()
		{
			var arg = _receivePool.PopFront();
			arg.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
			arg.Socket = this;

			if (!_socket.ReceiveAsync(arg))
			{
				//The receive was processed synchronously which means the callback wont be executed.
				OnReceiveComplete(arg);
			}
		}

		volatile int available = 0;
		volatile bool reset;

		protected void OnReceiveComplete(ReceiveEventArgs arg)
		{
			if (arg.SocketError != System.Net.Sockets.SocketError.Success)
			{
				//Release back to the pool
				arg.Socket = null;
				_receivePool.PushBack(arg);

				Close();
			}
			else if (arg.BytesTransferred == 0)
			{
				//Release back to the pool
				arg.Socket = null;
				_receivePool.PushBack(arg);

				Close();
			}
			else
			{
				//receivedBytes += arg.BytesTransferred;

				//lock (storeBuffer)
				{
					//var free = storeBuffer.Length - storeWriteOffset;
					//if (free < arg.BytesTransferred)
					//{
					//    throw new InvalidOperationException("Receive buffer has no room for this segment");
					//}

					//Migrate to the storeBuffer
					//Array.Copy(receiveBuffer, arg.Offset, storeBuffer, storeWriteOffset, arg.BytesTransferred);
					//storeWriteOffset += arg.BytesTransferred;

					lock (_rxSyncRoot)
					//lock (storeBuffer)
					{
						storeWriteOffset += arg.BytesTransferred;
						var free = arg.Buffer.Length - storeWriteOffset;

						if (free == 0)
						{
							//Move back to the start. This can overwrite data, so make sure terraria reading isn't slow!
							storeWriteOffset = 0;
							free = receiveBuffer.Length;
							reset = true;
							Console.WriteLine("Reader reset");
						}

						arg.SetBuffer(storeWriteOffset, free);

						available += arg.BytesTransferred;
					}
				}

				if (!_socket.ReceiveAsync(arg))
				{
					//The receive was processed synchronously which means the callback wont be executed.
					OnReceiveComplete(arg);
				}
			}
		}

		public void AsyncReceive(byte[] data, int offset, int count, SocketReceiveCallback callback, object state = null)
		{
			if (_connected && storeWriteOffset > 0)
			{
				//lock (storeBuffer)
				lock (_rxSyncRoot)
				{
					//var received = storeWriteOffset - storeReadOffset;
					int received;
					if (!reset)
					{
						received = storeWriteOffset - storeReadOffset;
					}
					else
					{
						received = receiveBuffer.Length - storeReadOffset;
						//reset = false;
					}

					if (count > received) count = received;
					if (count == 0) return;

					Array.Copy(receiveBuffer, storeReadOffset, data, offset, count);

					storeReadOffset += count;

					if (reset || storeReadOffset == receiveBuffer.Length)
					{
						storeReadOffset = 0;

						////The write buffer will also be at the end, so it must now also be reset
						//storeWriteOffset = 0;

						reset = false;
						Console.WriteLine("Queue reset");
					}

					////Recallibrate offsets
					//if (storeReadOffset + count >= storeWriteOffset)
					//{
					//    //We have caught up to the receive writer;
					//    storeReadOffset = 0;
					//    storeWriteOffset = 0;
					//}
					//else
					//{
					//    storeReadOffset += count;
					//}

					available -= count;
				}

				if (count > 0 && callback != null)
				{
					callback(state, count);
				}
			}
		}
	}
}
