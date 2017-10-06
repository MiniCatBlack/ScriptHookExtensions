using System;

namespace GTA.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IDisposable"/>.
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Adds a disposable to <see cref="ScriptEx.CompositeDisposable"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source"/>.</typeparam>
        /// <param name="source">A disposable object to add.</param>
        /// <param name="script">The reference of the target <see cref="ScriptEx"/>.</param>
        /// <returns>The same object as <paramref name="source"/>.</returns>
        public static T AddTo<T>(this T source, ScriptEx script) where T : IDisposable
        {
            if (source != null && script != null)
            {
                script.CompositeDisposable.Add(source);
            }

            return source;
        }
    }
}