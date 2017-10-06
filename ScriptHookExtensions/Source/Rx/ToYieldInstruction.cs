using System;
using System.Collections;
using System.Collections.Generic;

namespace GTA.Extensions
{
    /// <summary>
    /// Provides a way to treat an observable sequence as a coroutine.
    /// </summary>
    /// <typeparam name="T">The type of the source observable sequence.</typeparam>
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
        /// Gets a value that indicates whether the coroutine has an error.
        /// </summary>
        public bool HasError => Error != null;

        /// <summary>
        /// Gets an exception object.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the coroutine has a result.
        /// </summary>
        public bool HasResult { get; private set; }

        /// <summary>
        /// Gets the last element of the source observable sequence.
        /// </summary>
        public T Result { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the coroutine has already ended.
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
        /// Moves the enumerator until the source observable sequence notifies OnError or OnCompleted.
        /// </summary>
        /// <returns>A value that indicates whether the coroutine lasts.</returns>
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
        /// Unsubscribes the subscription of the source observable sequence.
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