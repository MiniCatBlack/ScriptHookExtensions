using System;

namespace GTA.Extensions
{
    /// <summary>
    /// Indicates the state of coroutine.
    /// </summary>
    public enum CoroutineState
    {
        /// <summary>
        /// The coroutine is running.
        /// </summary>
        Running,

        /// <summary>
        /// The coroutine is pausing.
        /// </summary>
        Pausing,

        /// <summary>
        /// The coroutine is finished.
        /// </summary>
        Finished
    }
}