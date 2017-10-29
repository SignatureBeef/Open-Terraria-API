using System;
using System.Collections.Generic;
using System.Threading;

namespace OTAPI.Sockets
{
	public class AsyncArgsPool<TSocketAsyncEventArgs>
		where TSocketAsyncEventArgs : AsyncSocketEventArgs, new()
	{
		private Queue<TSocketAsyncEventArgs> _pool = new Queue<TSocketAsyncEventArgs>();

		public long Leased
		{
			get
			{
				return Interlocked.Read(ref leased);
			}
		}

		public long Capacity { get; private set; }

		private long leased = 0;
		//private long id = 0;

		//private DateTime? lastReset = null;
		//private long packetsSinceReset = 0;
		public string Prefix { get; set; }

		public void PushBack(TSocketAsyncEventArgs args)
		{
			if (args.conn != null) throw new InvalidOperationException($"Cannot push in a non released socket. Please reset {nameof(args.conn)}");

			lock (_pool)
			{
				_pool.Enqueue(args);

				//packetsSinceReset++;
				//if (lastReset == null || (DateTime.Now - lastReset.Value).TotalMilliseconds >= 1000)
				//{
				//	System.Diagnostics.Debug.WriteLine($"[{Prefix} {typeof(TSocketAsyncEventArgs).Name}] {packetsSinceReset}");

				//	packetsSinceReset = 0;
				//	lastReset = DateTime.Now;
				//}
			}

			Interlocked.Decrement(ref leased);
		}

		public TSocketAsyncEventArgs PopFront()
		{
			Interlocked.Increment(ref leased);
			lock (_pool)
			{
				if (_pool.Count == 0)
				{
					Capacity++;
					System.Diagnostics.Debug.WriteLine($"[{Prefix} {typeof(TSocketAsyncEventArgs).Name} capacity now at {Leased}/{Capacity}");
					return new TSocketAsyncEventArgs();
				}

				return _pool.Dequeue();
			}
		}
	}
}
