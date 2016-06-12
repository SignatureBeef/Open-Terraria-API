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

        internal void Flush()
        {
            TrySend();
        }

        int txBytes = 0;
        static int txSegmentBytes = 2;

        protected void Send(Message message)
        {
            lock (_txQueue)
            {
                _sendQueue.Enqueue(message);
                _txQueue.Add(new ArraySegment<byte>(message.data));
            }

            txBytes += message.data.Length;

            TrySend();
        }

        int sendOffset;
        bool sending = false;

        protected void TrySend()
        {
            var bytes = txBytes;
            if (!sending && _txQueue.Count > 0)
            {
                SendData(null);
            }
        }

        public enum SendResult : int
        {
            NotQueued,
            Queued
        }

        protected SendResult SendData(SendEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"Sending data");
            if (args == null)
            {
                args = _sendPool.PopFront();
                if (args.Socket != null)
                    throw new InvalidOperationException($"{nameof(ReceiveEventArgs)} was not released correctly");

                args.Socket = this;
            }

            try
            {
                lock (_txQueue)
                {
                    if (_txQueue.Count == 0)
                    {
                        args.Socket = null;
                        _sendPool.PushBack(args);
                        return SendResult.NotQueued;
                    }

                    var local = new System.Collections.Generic.List<ArraySegment<byte>>(_txQueue); //debug
                    args.BufferList = local;

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

                    if (sent) return SendResult.Queued;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in {this.GetType().FullName}.{nameof(Flush)}\n{ex}");
            }

            return SendResult.NotQueued;
        }

        protected struct Message
        {
            public byte[] data;
            public SocketSendCallback callback;
            public object state;
        }

        protected virtual void OnSendComplete(SendEventArgs arg, System.Collections.Generic.List<Message> messages)
        {
            System.Diagnostics.Debug.WriteLine($"Sent {arg.BytesTransferred} bytes");

            foreach (var msg in messages)
                msg.callback(msg.state);

            if (SendData(arg) == SendResult.NotQueued)
            {
                sending = false;
                ////Release socket
                //arg.Socket = null;
                //_sendPool.PushBack(arg);
            }
        }
    }
}
