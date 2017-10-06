//  https://github.com/TORISOUP/UniRx/blob/a36ba7e6deeb2a544f4fd59eb91628c969384edb/Assets/Plugins/UniRx/Scripts/Operators/First.cs

using System;
using UniRx;
using UniRx.Operators;

namespace GTA.Extensions
{
    internal class FirstOrEmptyObservable<T> : OperatorObservableBase<T>
    {
        private readonly UniRx.IObservable<T> source;

        private readonly Func<T, bool> predicate;

        public FirstOrEmptyObservable(UniRx.IObservable<T> source) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        public FirstOrEmptyObservable(UniRx.IObservable<T> source, Func<T, bool> predicate) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
        }

        protected override IDisposable SubscribeCore(UniRx.IObserver<T> observer, IDisposable cancel)
        {
            if (predicate == null)
            {
                return source.Subscribe(new FirstOrEmpty(observer, cancel));
            }

            else
            {
                return source.Subscribe(new FirstOrEmpty_(this, observer, cancel));
            }
        }

        internal class FirstOrEmpty : OperatorObserverBase<T, T>
        {
            private bool notPublished = true;

            public FirstOrEmpty(UniRx.IObserver<T> observer, IDisposable cancel) : base(observer, cancel) { }

            public override void OnNext(T value)
            {
                if (notPublished)
                {
                    notPublished = false;
                    observer.OnNext(value);

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

            public override void OnError(Exception error)
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

            public override void OnCompleted()
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

        internal class FirstOrEmpty_ : OperatorObserverBase<T, T>
        {
            private readonly FirstOrEmptyObservable<T> parent;

            private bool notPublished;

            public FirstOrEmpty_(FirstOrEmptyObservable<T> parent, UniRx.IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                notPublished = true;
            }

            public override void OnNext(T value)
            {
                if (notPublished)
                {
                    bool isPassed;

                    try
                    {
                        isPassed = parent.predicate(value);
                    }

                    catch (Exception error)
                    {
                        try
                        {
                            observer.OnError(error);
                        }

                        finally
                        {
                            Dispose();
                        }

                        return;
                    }

                    if (isPassed)
                    {
                        notPublished = false;
                        observer.OnNext(value);

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

            public override void OnError(Exception error)
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

            public override void OnCompleted()
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