using System;
using System.Collections;
using System.Collections.Generic;

namespace GTA.Extensions
{
    /// <summary>
    /// Provides a way to treat an observable stream as a coroutine.
    /// </summary>
    /// <typeparam name="T">The type of the source stream.</typeparam>
    public class YieldInstruction<T> : IEnumerator<T>, IDisposable
    {
        private readonly IDisposable subscription;

        private readonly bool rethrowOnError;

        private bool moveNext = true;

        object IEnumerator.Current => Current;

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public T Current { get; private set; }

        /// <summary>
        /// Gets a boolean value that indicates whether the coroutine has an error.
        /// </summary>
        public bool HasError => Error != null;

        /// <summary>
        /// Gets the exception object that the stream notified.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets a boolean value that indicates whether the coroutine has a result.
        /// </summary>
        public bool HasResult { get; private set; }

        /// <summary>
        /// Gets the last value that the stream notified.
        /// </summary>
        public T Result { get; private set; }

        /// <summary>
        /// Gets a boolean value that indicates whether the coroutine ended.
        /// </summary>
        public bool IsDone => HasError || HasResult;

        internal YieldInstruction(UniRx.IObservable<T> source, bool rethrowOnError)
        {
            this.rethrowOnError = rethrowOnError;

            try
            {
                var observer = new ObserverYieldInstruction(this);
                subscription = source.Subscribe(observer);
            }

            catch
            {
                moveNext = false;
                throw;
            }
        }

        /// <summary>
        /// Moves the enumerator until the stream notifies OnError or OnCompleted.
        /// </summary>
        /// <returns>An boolean value that indicates whether the coroutine lasts.</returns>
        public bool MoveNext()
        {
            if (!moveNext)
            {
                if (rethrowOnError && HasError)
                {
                    throw Error;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Unsubscribes the subscription of the source stream.
        /// </summary>
        public void Dispose()
        {
            subscription.Dispose();
        }

        /// <summary>
        /// Throws a NotSupportedException.
        /// </summary>
        public void Reset() => throw new NotSupportedException();

        internal class ObserverYieldInstruction : UniRx.IObserver<T>
        {
            private readonly YieldInstruction<T> parent;

            public ObserverYieldInstruction(YieldInstruction<T> parent)
            {
                this.parent = parent;
            }

            public void OnNext(T value)
            {
                parent.Current = value;
            }

            public void OnError(Exception error)
            {
                parent.moveNext = false;
                parent.Error = error;
            }

            public void OnCompleted()
            {
                parent.moveNext = false;
                parent.Result = parent.Current;
                parent.HasResult = true;
            }
        }
    }
}