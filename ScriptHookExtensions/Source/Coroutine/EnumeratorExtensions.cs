using System;
using System.Collections;

namespace GTA.Extensions
{
    internal static class EnumeratorExtensions
    {
        public static IEnumerator ToCoroutine(this IEnumerator source)
        {
            try
            {
                while (source.MoveNext())
                {
                    var inner = (source.Current as InnerCoroutine)?.Enumerator ?? source.Current as IEnumerator;
                    if (inner != null)
                    {
                        try
                        {
                            while (inner.MoveNext())
                            {
                                yield return inner.Current;
                            }
                        }

                        finally
                        {
                            (inner as IDisposable)?.Dispose();
                        }
                    }

                    yield return source.Current;
                }
            }

            finally
            {
                (source as IDisposable)?.Dispose();
            }
        }
    }
}