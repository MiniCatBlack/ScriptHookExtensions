using System;
using UniRx;

namespace GTA.Extensions
{
    internal class ScheduledItem : IComparable<ScheduledItem>
    {
        private Action action;

        private readonly BooleanDisposable cancellation = new BooleanDisposable();

        public bool IsCancelled => cancellation.IsDisposed;

        public TimeSpan DueTime { get; }

        public IDisposable Cancellation => cancellation;

        public ScheduledItem(TimeSpan dueTime, Action action)
        {
            this.action = action;
            DueTime = dueTime;
        }

        public int CompareTo(ScheduledItem other)
        {
            if (ReferenceEquals(other, null)) return 1;

            return DueTime.CompareTo(other.DueTime);
        }

        public void Invoke()
        {
            if (!cancellation.IsDisposed)
            {
                action?.Invoke();
            }
        }
    }
}