using System;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
    //Sending
    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {

        //private System.Collections.Concurrent.ConcurrentQueue<Message> confirmQueue = new System.Collections.Concurrent.ConcurrentQueue<Message>();
        private System.Collections.Generic.Queue<Message> _sendQueue = new System.Collections.Generic.Queue<Message>();
        private System.Collections.Generic.List<ArraySegment<byte>> _txQueue = new System.Collections.Generic.List<ArraySegment<byte>>();

        public void AsyncSend(byte[] data, int offset, int count, SocketSendCallback callback, object state = null)
        {
            var bytes = new byte[count];
            Array.Copy(data, offset, bytes, 0, count);
            Send(new Message()
            {
                data = bytes,
                callback = callback,
                state = state
            });
        }

        static int SegmentSize = 50;
        internal void Flush()
        {
            TrySend(true);
        }

        volatile int txBytes = 0;
        static int txSegmentBytes = 1024;

        protected void Send(Message message, bool flush = false)
        {
            lock (_txQueue)
            {
                _sendQueue.Enqueue(message);
                _txQueue.Add(new ArraySegment<byte>(message.data));
            }

            txBytes += message.data.Length;

            TrySend(flush);
        }

        volatile int sendOffset;
        volatile bool sending = false;

        protected void TrySend(bool flush = false)
        {
            var bytes = txBytes;
            if (!sending && (bytes >= txSegmentBytes || flush) && _txQueue.Count > 0)
            {
                SendData();
            }
        }

        protected void SendData()
        {
            var args = _sendPool.PopFront();
            if (args.Socket != null)
                throw new InvalidOperationException($"{nameof(ReceiveEventArgs)} was not released correctly");

            args.Socket = this;
            try
            {
                lock (_txQueue)
                {
                    args.BufferList = _txQueue;

                    var callbacks = new System.Collections.Generic.List<Message>();
                    while (_sendQueue.Count > 0)
                        callbacks.Add(_sendQueue.Dequeue());

                    args.UserToken = callbacks;

                    sending = true;
                    var sent = _socket.SendAsync(args);
                    if (!sent)
                    {
                        OnSendComplete(args, callbacks);
                    }
                    _txQueue.Clear();
                    sending = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in {this.GetType().FullName}.{nameof(Flush)}\n{ex}");
            }
        }

        protected struct Message
        {
            public byte[] data;
            public SocketSendCallback callback;
            public object state;
        }
    }
}
