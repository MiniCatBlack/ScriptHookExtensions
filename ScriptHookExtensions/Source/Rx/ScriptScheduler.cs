using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniRx;

namespace GTA.Extensions
{
    internal class ScriptScheduler : IScheduler
    {
        private readonly object lockObject = new object();

        private readonly Queue<Action> queue = new Queue<Action>();

        private readonly PriorityQueue<ScheduledItem> scheduledQueue = new PriorityQueue<ScheduledItem>();

        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public DateTimeOffset Now => Scheduler.Now;

        public IDisposable Schedule(Action action)
        {
            if (action != null)
            {
                lock (lockObject)
                {
                    queue.Enqueue(action);
                }
            }

            return Disposable.Empty;
        }

        public IDisposable Schedule(TimeSpan dueTime, Action action)
        {
            if (action != null)
            {
                if ((dueTime = Scheduler.Normalize(dueTime)) == TimeSpan.Zero)
                    Schedule(action);

                var item = new ScheduledItem(dueTime, action);

                lock (lockObject)
                {
                    scheduledQueue.Enqueue(item);
                }

                return item.Cancellation;
            }

            return Disposable.Empty;
        }

        internal void Run()
        {
            if (queue.Count > 0)
            {
                lock (lockObject)
                {
                    while (queue.Count > 0)
                    {
                        queue.Dequeue().Invoke();
                    }
                }
            }

            if (scheduledQueue.Count > 0)
            {
                lock (lockObject)
                {
                    while (scheduledQueue.Count > 0)
                    {
                        var item = scheduledQueue.Peek();

                        if (item.IsCancelled)
                        {
                            scheduledQueue.Dequeue();
                            continue;
                        }

                        var t = item.DueTime - stopwatch.Elapsed;
                        if (t.Ticks <= 0)
                        {
                            item.Invoke();
                            scheduledQueue.Dequeue();
                            continue;
                        }

                        break;
                    }
                }
            }
        }
    }
}