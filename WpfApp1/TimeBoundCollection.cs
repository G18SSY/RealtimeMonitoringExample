using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace WpfApp1
{
    public class TimeBoundCollection<T> : IReadOnlyCollection<T>
    {
        private readonly SemaphoreSlim autoDequeueSemaphore = new(1);
        private readonly TimeSpan duration;
        private readonly ConcurrentQueue<T> queue = new();

        private readonly Func<T, DateTime> timestampCallback;

        public TimeBoundCollection(Func<T, DateTime> timestampCallback, TimeSpan duration)
        {
            this.timestampCallback = timestampCallback;
            this.duration = duration;
        }

        public IEnumerator<T> GetEnumerator()
            => queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)queue).GetEnumerator();

        public int Count => queue.Count;

        public void Add(T value)
        {
            queue.Enqueue(value);

            DequeueLapsedSafe();
        }

        private void DequeueLapsedSafe()
        {
            // If a dequeue is already in progress then no point waiting to do another
            if (!autoDequeueSemaphore.Wait(0))
                return;

            try
            {
                DateTime timeout = DateTime.Now - duration;

                while (!queue.IsEmpty && queue.TryPeek(out T? peeked) && timestampCallback(peeked) < timeout)
                {
                    if (queue.TryDequeue(out _))
                        // If the dequeue failed for some reason then return otherwise we could get stuck in this loop
                        return;
                }
            }
            finally
            {
                autoDequeueSemaphore.Release();
            }
        }

        public void Clear()
        {
            queue.Clear();
        }
    }
}