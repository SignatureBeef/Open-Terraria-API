using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace OTAPI.Sockets
{
    public class PoolSocketEventArgs : SocketAsyncEventArgs
    {
        public PoolSocket Socket { get; set; }
    }

    public class ArgsPool<TSocketAsyncEventArgs>
        where TSocketAsyncEventArgs : PoolSocketEventArgs, new()
    {
        private Queue<TSocketAsyncEventArgs> _pool = new Queue<TSocketAsyncEventArgs>();

        public int Capacity { get; private set; }

        public void PushBack(TSocketAsyncEventArgs args)
        {
            if (args.Socket != null) throw new InvalidOperationException("Cannot push in a non released socket");

            lock (_pool)
            {
                _pool.Enqueue(args);
            }
        }

        public TSocketAsyncEventArgs PopFront()
        {
            lock (_pool)
            {
                if (_pool.Count == 0)
                {
                    Capacity++;
                    //System.Diagnostics.Debug.WriteLine($"{this.GetType().Name} capacity now at {Capacity}");
                    return new TSocketAsyncEventArgs();
                }

                return _pool.Dequeue();
            }
        }
    }
}
