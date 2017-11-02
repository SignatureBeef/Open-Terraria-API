using System;
using System.Collections.Generic;
using System.Threading;

namespace OTAPI.Sockets
{
	public class AsyncArgsPool<TSocketAsyncEventArgs>
		where TSocketAsyncEventArgs : AsyncSocketEventArgs, new()
	{
		private Queue<TSocketAsyncEventArgs> _pool = new Queue<TSocketAsyncEventArgs>();

		public int Capacity => Interlocked.CompareExchange(ref capacity, 0, 0);

		private int free = 0;
		private int capacity = 0;

		public string Prefix { get; set; }

		public AsyncArgsPool(string prefix)
		{
			this.Prefix = prefix;
		}

		public TSocketAsyncEventArgs PopFront()
		{
			var free_args = Interlocked.CompareExchange(ref free, 0, 0);
			if (free_args <= 0)
			{
				Console.WriteLine($"[{Prefix} {typeof(TSocketAsyncEventArgs).Name}] capacity now at: {Capacity}");
				return new TSocketAsyncEventArgs();
			}

			return _pool.Dequeue();
		}

		public void PushBack(TSocketAsyncEventArgs args)
		{
			if (args.conn != null) throw new InvalidOperationException($"Cannot push in a non released socket. Please reset {nameof(args.conn)}");

			_pool.Enqueue(args);

			Interlocked.Increment(ref free);
		}
	}
}
