using System;
using System.Collections;
using UniRx;

namespace GTA.Extensions
{
    /// <summary>
    /// Provides extension methods for IObservable.
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Converts the specified stream to a coroutine that stops when the stream notifies OnError or OnCompleted.
        /// </summary>
        /// <typeparam name="T">The type of the specified stream.</typeparam>
        /// <param name="source">An observable stream to convert to a coroutine.</param>
        /// <param name="rethrowOnError">A flag that indicates whether the coroutine throws an exception if the stream notifies OnError.</param>
        /// <returns>A coroutine that stops when the source stream notifies OnError or Oncompleted.</returns>
        public static IEnumerator ToYieldInstruction<T>(this UniRx.IObservable<T> source, bool rethrowOnError = false)
        {
            return new ToYieldInstruction<T>(source, rethrowOnError);
        }
    }
}