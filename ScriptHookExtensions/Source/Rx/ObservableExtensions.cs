using System;
using UniRx;

namespace GTA.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="UniRx.IObservable{T}"/>.
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Converts the specified observable sequence to a coroutine that lasts until the sequence notifies OnError or OnCompleted.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source"/>.</typeparam>
        /// <param name="source">An observable sequence to convert to a coroutine.</param>
        /// <param name="rethrowOnError">A value that indicates whether the coroutine throws an exception if the source observable sequence notifies OnError.</param>
        /// <returns>A coroutine that is converted from the specified observable sequence.</returns>
        public static YieldInstruction<T> ToYieldInstruction<T>(this UniRx.IObservable<T> source, bool rethrowOnError = false)
        {
            return new YieldInstruction<T>(source, rethrowOnError);
        }

        /// <summary>
        /// Returns the specified value if no value is notified while the specified dueTime.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source"/>.</typeparam>
        /// <param name="source">The source observable sequence.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="dueTime">The duration time.</param>
        /// <param name="scheduler">The scheduler to run the timers on.</param>
        /// <returns>An observable sequence that notifies the specified value if no value is notified while the specified duration time.</returns>
        public static UniRx.IObservable<T> ResetAfter<T>(this UniRx.IObservable<T> source, T defaultValue, TimeSpan dueTime, IScheduler scheduler)
        {
            return new ResetAfterObservable<T>(source, defaultValue, dueTime, scheduler);
        }

        /// <summary>
        /// Returns the specified value if no value is notified while the specified dueTime.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source"/>.</typeparam>
        /// <param name="source">The source observable sequence.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="dueTime">The duration time.</param>
        /// <returns>An observable sequence that notifies the specified value if no value is notified while the specified duration time.</returns>
        public static UniRx.IObservable<T> ResetAfter<T>(this UniRx.IObservable<T> source, T defaultValue, TimeSpan dueTime)
        {
            return source.ResetAfter(defaultValue, dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        /// <summary>
        /// Returns the default value of the specified type parameter if no value is notified while the specified dueTime.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source"/>.</typeparam>
        /// <param name="source">The source observable sequence.</param>
        /// <param name="dueTime">The duration time.</param>
        /// <returns>An observable sequence that notifies the default value if no value is notified while the specified duration time.</returns>
        public static UniRx.IObservable<T> ResetAfter<T>(this UniRx.IObservable<T> source, TimeSpan dueTime)
        {
            return source.ResetAfter(default(T), dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        /// <summary>
        /// Returns the first element of an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source"/>.</typeparam>
        /// <param name="source">The source observable sequence.</param>
        /// <returns>An observable sequence that notifies the first element if exists.</returns>
        public static UniRx.IObservable<T> FirstOrEmpty<T>(this UniRx.IObservable<T> source)
        {
            return new FirstOrEmptyObservable<T>(source);
        }

        /// <summary>
        /// Returns the first element of an observable sequence that matches the predicate.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source"/>.</typeparam>
        /// <param name="source">The source observable sequence.</param>
        /// <param name="predicate">A predicate function to evaluate for elements in the sequence.</param>
        /// <returns>An observable sequence that notifies the first element that matches the predicate if exists.</returns>
        public static UniRx.IObservable<T> FirstOrEmpty<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
        {
            return new FirstOrEmptyObservable<T>(source, predicate);
        }
    }
}