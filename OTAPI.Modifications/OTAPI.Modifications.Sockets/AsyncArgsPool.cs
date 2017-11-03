using System;
using System.Collections.Concurrent;

namespace OTAPI.Sockets
{
	public class AsyncArgsPool<TSocketAsyncEventArgs>
		where TSocketAsyncEventArgs : AsyncSocketEventArgs, new()
	{
		private ConcurrentStack<TSocketAsyncEventArgs> _pool = new ConcurrentStack<TSocketAsyncEventArgs>();

		public string Prefix { get; set; }

		public AsyncArgsPool(string prefix)
		{
			this.Prefix = prefix;
		}

		public TSocketAsyncEventArgs PopFront()
		{
			TSocketAsyncEventArgs args;

			if (!_pool.TryPop(out args))
			{
				args = new TSocketAsyncEventArgs();
			}
			return args;
		}

		public void PushBack(TSocketAsyncEventArgs args)
		{
			if (args.conn != null) throw new InvalidOperationException($"Cannot push in a non released socket. Please reset {nameof(args.conn)}");

			_pool.Push(args);
		}
	}
}
