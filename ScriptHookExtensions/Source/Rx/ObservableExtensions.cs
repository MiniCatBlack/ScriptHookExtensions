using System;
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
        public static YieldInstruction<T> ToYieldInstruction<T>(this UniRx.IObservable<T> source, bool rethrowOnError = false)
        {
            return new YieldInstruction<T>(source, rethrowOnError);
        }

        /// <summary>
        /// Returns a stream that notifies the default value if no value is notified while the specified dueTime.
        /// </summary>
        /// <typeparam name="T">The type of notification value.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="defaultValue">The default value to notify.</param>
        /// <param name="dueTime">The duration time to wait</param>
        /// <param name="scheduler">The scheduler to run the timers on.</param>
        /// <returns>A new observable sequence.</returns>
        public static UniRx.IObservable<TSource> ResetAfter<TSource>(this UniRx.IObservable<TSource> source, TSource defaultValue, TimeSpan dueTime, IScheduler scheduler)
        {
            return new ResetAfterObservable<TSource>(source, defaultValue, dueTime, scheduler);
        }

        /// <summary>
        /// Returns a stream that notifies the default value if no value is notified while the specified dueTime.
        /// </summary>
        /// <typeparam name="T">The type of notification value.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="defaultValue">The default value to notify.</param>
        /// <param name="dueTime">The duration time to wait</param>
        /// <returns>A new observable sequence.</returns>
        public static UniRx.IObservable<TSource> ResetAfter<TSource>(this UniRx.IObservable<TSource> source, TSource defaultValue, TimeSpan dueTime)
        {
            return source.ResetAfter(defaultValue, dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        /// <summary>
        /// Returns a stream that notifies the default value if no value is notified while the specified dueTime.
        /// </summary>
        /// <typeparam name="T">The type of notification value.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="dueTime">The duration time to wait</param>
        /// <returns>A new observable sequence.</returns>
        public static UniRx.IObservable<TSource> ResetAfter<TSource>(this UniRx.IObservable<TSource> source, TimeSpan dueTime)
        {
            return source.ResetAfter(default(TSource), dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static UniRx.IObservable<T> FirstOrEmpty<T>(this UniRx.IObservable<T> source)
        {
            return new FirstOrEmptyObservable<T>(source);
        }

        public static UniRx.IObservable<T> FirstOrEmpty<T>(this UniRx.IObservable<T> source, Func<T, bool> predicate)
        {
            return new FirstOrEmptyObservable<T>(source, predicate);
        }
    }
}