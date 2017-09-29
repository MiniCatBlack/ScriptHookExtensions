using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace GTA.Extensions
{
    /// <summary>
    /// Provides the functionality for coroutine-based scripting.
    /// </summary>
    public class CoroutineManager
    {
        private readonly object lockObject = new object();

        private readonly LinkedList<CoroutineIterator> list = new LinkedList<CoroutineIterator>();

        private PrimaryCoroutine current;

        private bool IsRunning => current != null;

        /// <summary>
        /// Gets the integer that indicates the number of active coroutines.
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        /// Start the specific coroutine.
        /// </summary>
        /// <param name="coroutine">A coroutine to start.</param>
        /// <returns>The CoroutineHandle to handle the specified coroutine.</returns>
        public CoroutineHandle Start(IEnumerable coroutine)
            => Start(coroutine.GetEnumerator());

        /// <summary>
        /// Start the specific coroutine.
        /// </summary>
        /// <param name="coroutine">A coroutine to start.</param>
        /// <returns>The CoroutineHandle to handle the specified coroutine.</returns>
        public CoroutineHandle Start(IEnumerator coroutine)
        {
            if (IsRunning)
            {
                return new InnerCoroutine(coroutine.ToCoroutine(), current);
            }

            var handle = new PrimaryCoroutine();

            lock (lockObject)
            {
                var iterator = new CoroutineIterator(coroutine.ToCoroutine(), handle);
                list.AddLast(iterator);
            }

            return handle;
        }

        /// <summary>
        /// Converts the specified coroutine to IObservable.
        /// </summary>
        /// <typeparam name="T">The type of the notification value.</typeparam>
        /// <param name="coroutine">A coroutine to convert.</param>
        /// <returns>An observable object that converted from the specified coroutine.</returns>
        public UniRx.IObservable<T> ToObservable<T>(IEnumerable coroutine)
            => ToObservable<T>(coroutine.GetEnumerator);

        /// <summary>
        /// Converts the specified coroutine to IObservable.
        /// </summary>
        /// <typeparam name="T">The type of the notification value.</typeparam>
        /// <param name="coroutine">A coroutine to convert.</param>
        /// <returns>An observable object that converted from the specified coroutine.</returns>
        public UniRx.IObservable<T> ToObservable<T>(Func<IEnumerator> coroutine)
            => new ToObservable<T>(this, coroutine);

        /// <summary>
        /// Waits for the specified dueTime.
        /// </summary>
        /// <param name="dueTime">The relative time for which the coroutine waits.</param>
        /// <returns>An inner-coroutine to be used in the parent coroutine.</returns>
        public IEnumerator Wait(TimeSpan dueTime)
        {
            var stopwatch = Stopwatch.StartNew();
            var totalMillisec = dueTime.TotalMilliseconds;

            while (stopwatch.ElapsedMilliseconds < totalMillisec)
            {
                yield return null;
            }

            yield break;
        }

        /// <summary>
        /// Waits for the specified dueFrame.
        /// </summary>
        /// <param name="dueFrame">The number of the frame for which the coroutine waits.</param>
        /// <returns>An inner-coroutine to be used in the parent coroutine.</returns>
        public IEnumerator Wait(int dueFrame)
        {
            for (int i = 0; i < dueFrame; i++)
            {
                yield return null;
            }

            yield break;
        }

        internal void Run()
        {
            lock (lockObject)
            {
                var removeList = new LinkedList<LinkedListNode<CoroutineIterator>>();

                for (var node = list.First; node != null; node = node.Next)
                {
                    var iterator = node.Value;
                    current = iterator.Handle;

                    if (!iterator.Update())
                    {
                        iterator.Dispose();
                        removeList.AddLast(node);
                    }
                }
                current = null;

                if (removeList.Count > 0)
                {
                    for (var node = removeList.First; node != null; node = node.Next)
                    {
                        list.Remove(node.Value);
                    }
                }
            }
        }
    }
}