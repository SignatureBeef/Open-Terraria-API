using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
    //Sending
    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {

        //private System.Collections.Concurrent.ConcurrentQueue<Message> confirmQueue = new System.Collections.Concurrent.ConcurrentQueue<Message>();
        private Queue<Message> _sendQueue = new Queue<Message>();
        //private Queue<ArraySegment<byte>> _txQueue = new Queue<ArraySegment<byte>>();
        private List<ArraySegment<byte>> _txQueue = new List<ArraySegment<byte>>();
        private readonly object _txSyncRoot = new object();

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
        int txOffset = 0;
        static int txSegmentBytes = 2;

        protected void Send(Message message)
        {
            lock (_txSyncRoot)
            {
                _sendQueue.Enqueue(message);
                _txQueue.Add(new ArraySegment<byte>(message.data));
            }

            txBytes += message.data.Length;

            TrySend();
        }

        void ISocket.SendQueuedPackets()
        {
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
            if (args == null)
            {
                args = _sendPool.PopFront();
                if (args.Socket != null)
                    throw new InvalidOperationException($"{nameof(ReceiveEventArgs)} was not released correctly");

                args.Socket = this;

                //Create the buffer list that we use to store our queued data.
                if (args.BufferList == null)
                    args.BufferList = new List<ArraySegment<byte>>();
            }

            //Ensure our operation collections are reset
            if (args.BufferList.Count > 0) args.BufferList.Clear();
            if (args.Confirmations.Count > 0) args.Confirmations.Clear();

            try
            {
                lock (_txSyncRoot)
                {
                    if (_txQueue.Count == 0)
                    {
                        args.Socket = null;
                        _sendPool.PushBack(args);
                        return SendResult.NotQueued;
                    }
                    
                    //Let our data list be sent out
                    args.BufferList = _txQueue;
                    _txQueue = new List<ArraySegment<byte>>();
                    
                    //Place all callbacks into this operation
                    while (_sendQueue.Count > 0)
                        args.Confirmations.Add(_sendQueue.Dequeue());

                    if (!sending) sending = true;
                    var sent = _socket.SendAsync(args);
                    if (!sent)
                    {
                        OnSendComplete(args);
                    }

                    if (sent) return SendResult.Queued;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in {this.GetType().FullName}.{nameof(SendData)}\n{ex}");
            }

            return SendResult.NotQueued;
        }

        public struct Message
        {
            public byte[] data;
            public SocketSendCallback callback;
            public object state;
        }

        protected virtual void OnSendComplete(SendEventArgs arg)
        {
            if (arg.SocketError != System.Net.Sockets.SocketError.Success)
            {
                //Release back to the pool
                arg.Socket = null;
                _sendPool.PushBack(arg);
                sending = false;

                Close();
            }
            else if (arg.BytesTransferred == 0)
            {
                //Release back to the pool
                arg.Socket = null;
                _sendPool.PushBack(arg);
                sending = false;

                Close();
            }
            else
            {
                foreach (var msg in arg.Confirmations)
                    msg.callback(msg.state);

                if (SendData(arg) == SendResult.NotQueued)
                {
                    sending = false;
                }
            }
        }
    }
}
