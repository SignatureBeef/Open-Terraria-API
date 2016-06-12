using System;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {
        private byte[] receiveBuffer;
        private byte[] storeBuffer;
        private int storeWriteOffset = 0;
        private int storeReadOffset = 0;

        protected void StartReceiving()
        {
            if (receiveBuffer != null)
                throw new InvalidOperationException("Receive buffer already initialised");

            receiveBuffer = new byte[1024]; //Receives packets
            storeBuffer = new byte[1024]; //Stores the data, ready for the AsyncReceive method to request data

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

                lock (storeBuffer)
                {
                    var free = storeBuffer.Length - storeWriteOffset;
                    if (free < arg.BytesTransferred)
                    {
                        throw new InvalidOperationException("Receive buffer has no room for this segment");
                    }

                    //Migrate to the storeBuffer
                    Array.Copy(receiveBuffer, arg.Offset, storeBuffer, storeWriteOffset, arg.BytesTransferred);
                    storeWriteOffset += arg.BytesTransferred;
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
                //System.Diagnostics.Debug.WriteLine("Data requested");
                lock (storeBuffer)
                {
                    var available = storeWriteOffset - storeReadOffset;
                    if (count > available) count = available;

                    Array.Copy(storeBuffer, storeReadOffset, data, offset, count);

                    //Recallibrate offsets
                    if (storeReadOffset + count >= storeWriteOffset)
                    {
                        //We have caught up to the receive writer;
                        storeReadOffset = 0;
                        storeWriteOffset = 0;
                    }
                    else
                    {
                        storeReadOffset += count;
                    }
                }

                if (count > 0 && callback != null)
                {
                    callback(state, count);
                }
            }
        }
    }
}
