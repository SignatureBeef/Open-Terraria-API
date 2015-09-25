using System;
using System.Collections.Concurrent;

namespace OTA.Misc
{
    /// <summary>
    /// Fixed-size concurrent queue.
    /// </summary>
    public class FixedConcurrentQueue<T> : ConcurrentQueue<T>
    {
        /// <summary>
        /// Gets the max size of the queue
        /// </summary>
        /// <value>The size of the max.</value>
        public int MaxSize { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OTA.Misc.FixedConcurrentQueue`1"/> class.
        /// </summary>
        /// <param name="maxSize">Max size of the queue.</param>
        public FixedConcurrentQueue(int maxSize)
        {
            MaxSize = maxSize;
        }

        readonly object _enqueueLock = new object();
        public new void Enqueue(T item)
        {
            lock (_enqueueLock)
            {
                base.Enqueue(item);

                while (Count > MaxSize)
                {
                    T tmp;
                    TryDequeue(out tmp);
                }
            }
        }
    }
}

