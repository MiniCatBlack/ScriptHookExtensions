//  https://github.com/TORISOUP/UniRx/blob/13ea83b38c1bc3bede700a7da17e60f67c3c5f08/Dlls/UniRx.Library/Operators/ResetAfter.cs

using System;
using UniRx;
using UniRx.Operators;

namespace GTA.Extensions
{
    internal class ResetAfterObservable<T> : OperatorObservableBase<T>
    {
        private readonly UniRx.IObservable<T> source;

        private readonly TimeSpan dueTime;

        private readonly IScheduler scheduler;

        private readonly T defaultValue;

        public ResetAfterObservable(UniRx.IObservable<T> source, T defaultValue, TimeSpan dueTime, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTime = dueTime;
            this.scheduler = scheduler;
            this.defaultValue = defaultValue;
        }

        protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
        {
            return new ResetAfter(this, observer, cancel).Run();
        }

        internal class ResetAfter : OperatorObserverBase<T, T>
        {
            private readonly ResetAfterObservable<T> parent;

            private readonly object gate = new object();

            private SerialDisposable cancellable;

            public ResetAfter(ResetAfterObservable<T> parent, UniRx.IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                cancellable = new SerialDisposable();
                var subscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(cancellable, subscription);
            }

            private void OnNext()
            {
                lock (gate)
                {
                    observer.OnNext(parent.defaultValue);
                }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    observer.OnNext(value);

                    var d = new SingleAssignmentDisposable();
                    cancellable.Disposable = d;
                    d.Disposable = parent.scheduler.Schedule(parent.dueTime, OnNext);
                }
            }

            public override void OnError(Exception error)
            {
                cancellable.Dispose();

                lock (gate)
                {
                    try
                    {
                        observer.OnError(error);
                    }

                    finally
                    {
                        Dispose();
                    }
                }
            }

            public override void OnCompleted()
            {
                cancellable.Dispose();

                lock (gate)
                {
                    try
                    {
                        observer.OnCompleted();
                    }

                    finally
                    {
                        Dispose();
                    }
                }
            }
        }
    }
}