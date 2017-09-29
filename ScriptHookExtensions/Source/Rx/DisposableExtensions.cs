using System;

namespace GTA.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IDisposable"/>.
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Adds the source disposable to the <see cref="ScriptEx.CompositeDisposable"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The target disposable to add.</param>
        /// <param name="script">The target <see cref="ScriptEx"/>.</param>
        /// <returns>The same object as source.</returns>
        public static TSource AddTo<TSource>(this TSource source, ScriptEx script) where TSource : IDisposable
        {
            if (source != null && script != null)
            {
                script.CompositeDisposable.Add(source);
            }

            return source;
        }
    }
}