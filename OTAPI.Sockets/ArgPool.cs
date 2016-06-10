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
        private Stack<TSocketAsyncEventArgs> _pool = new Stack<TSocketAsyncEventArgs>();

        public int Capacity { get; private set; }

        public void Fill(TSocketAsyncEventArgs args)
        {
            if (args.Socket == null)
            {
                return;
            }

            lock (_pool)
            {
                args.Socket = null;
                _pool.Push(args);
            }
        }

        public TSocketAsyncEventArgs Drain()
        {
            lock(_pool)
            {
                if (_pool.Count == 0)
                {
                    Capacity++;
                    System.Diagnostics.Debug.WriteLine($"{this.GetType().Name} capacity now at {Capacity}");
                    return new TSocketAsyncEventArgs();
                }

                return _pool.Pop();
            }
        }
    }
}
