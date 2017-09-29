using System;

namespace GTA.Extensions
{
    /// <summary>
    /// Provides methods to handle the coroutine.
    /// </summary>
    public abstract class CoroutineHandle
    {
        /// <summary>
        /// Gets the state of the coroutine.
        /// </summary>
        public abstract CoroutineState State { get; internal set; }

        /// <summary>
        /// Pauses the coroutine.
        /// </summary>
        public abstract void Pause();

        /// <summary>
        /// Resumes the coroutine.
        /// </summary>
        public abstract void Resume();

        /// <summary>
        /// Stops the coroutine.
        /// </summary>
        public abstract void Kill();
    }
}