using System;
using System.Collections;
using System.Collections.Generic;

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