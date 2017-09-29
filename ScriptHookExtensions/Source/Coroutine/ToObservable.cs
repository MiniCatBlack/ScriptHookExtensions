using System;
using System.Collections;
using UniRx;

namespace GTA.Extensions
{
    internal class ToObservable<T> : UniRx.IObservable<T>
    {
        private readonly CoroutineManager manager;

        private readonly Func<IEnumerator> coroutine;

        internal ToObservable(CoroutineManager manager, Func<IEnumerator> coroutine)
        {
            this.manager = manager;
            this.coroutine = coroutine;
        }

        public IDisposable Subscribe(UniRx.IObserver<T> observer)
        {
            if (observer == null) return Disposable.Empty;

            var enumerator = new ToObservableEnumerator(coroutine().ToCoroutine(), observer);
            manager.Start(enumerator);

            return enumerator.Cancellation;
        }

        internal class ToObservableEnumerator : IEnumerator, IDisposable
        {
            private readonly IEnumerator coroutine;

            private UniRx.IObserver<T> observer;

            public object Current { get; private set; }

            public BooleanDisposable Cancellation { get; } = new BooleanDisposable();

            public ToObservableEnumerator(IEnumerator coroutine, UniRx.IObserver<T> observer)
            {
                this.coroutine = coroutine;
                this.observer = observer;
            }

            public bool MoveNext()
            {
                try
                {
                    //  OnCompleted
                    if (Cancellation.IsDisposed || !coroutine.MoveNext())
                    {
                        observer.OnCompleted();
                        return false;
                    }

                    else
                    {
                        Current = coroutine.Current;

                        //  OnNext
                        if (Current is T value)
                        {
                            observer.OnNext(value);
                        }

                        //  OnError
                        else if (Current is Exception error)
                        {
                            observer.OnError(error);
                            return false;
                        }

                        return true;
                    }
                }

                //  OnError
                catch (Exception error)
                {
                    observer.OnError(error);
                    return false;
                }
            }

            public void Dispose()
            {
                (coroutine as IDisposable)?.Dispose();
                observer = null;
            }

            public void Reset() => throw new NotSupportedException();
        }
    }
}