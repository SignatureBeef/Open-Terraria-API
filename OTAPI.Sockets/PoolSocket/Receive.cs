using System;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
    //Receiving
    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {
        private byte[] receiveBuffer;
        private byte[] storeBuffer;
        private bool receiving;
        //private volatile int receivedBytes = 0;
        private volatile int storeWriteOffset = 0;
        private volatile int storeReadOffset = 0;

        protected void StartReceiving()
        {
            if (receiveBuffer != null)
                throw new InvalidOperationException("Receive buffer already initialised");
            if (receiving)
                throw new InvalidOperationException("Receiving has already been started");

            receiveBuffer = new byte[4096];
            storeBuffer = new byte[4096];

            ReceiveData();
        }

        private void ReceiveData()
        {
            receiving = true;

            var arg = _receivePool.PopFront();
            arg.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
            arg.Socket = this;

            System.Diagnostics.Debug.WriteLine("Receiving more");
            if (!_socket.ReceiveAsync(arg))
            {
                //The receive was processed synchronously which means the callback wont be executed.
                OnReceiveComplete(arg);
            }

        }

        protected void OnReceiveComplete(ReceiveEventArgs arg)
        {
            System.Diagnostics.Debug.WriteLine($"Recieving {arg.BytesTransferred} bytes");
            receiving = false;
            //Check for closures 

            //receivedBytes += arg.BytesTransferred;

            lock (storeBuffer)
            {
                //Migrate to the storeBuffer
                Array.Copy(receiveBuffer, arg.Offset, storeBuffer, storeWriteOffset, arg.BytesTransferred);
                storeWriteOffset += arg.BytesTransferred;
            }

            //Release back to the pool
            arg.Socket = null;
            _receivePool.PushBack(arg);

            if (!receiving)
            {
                ReceiveData();
            }
        }

        public void AsyncReceive(byte[] data, int offset, int count, SocketReceiveCallback callback, object state = null)
        {
            //var max = 0;
            lock (storeBuffer)
            {
                //max = count;
                //if (max > storeWriteOffset) max = storeWriteOffset - storeReadOffset;
                var available = storeWriteOffset - storeReadOffset;
                if(count > available ) count = available;

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
