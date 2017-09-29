using System;
using System.Collections;

namespace GTA.Extensions
{
    internal class CoroutineIterator : IDisposable
    {
        private readonly IEnumerator coroutine;

        public PrimaryCoroutine Handle { get; }

        public CoroutineIterator(IEnumerator coroutine, PrimaryCoroutine handle)
        {
            this.coroutine = coroutine;
            Handle = handle;
        }

        private bool MoveCoroutine()
        {
            try
            {
                if (!coroutine.MoveNext())
                {
                    Handle.State = CoroutineState.Finished;
                    return false;
                }

                return true;
            }

            catch
            {
                Handle.State = CoroutineState.Finished;
                return false;
            }
        }

        public bool Update()
        {
            switch (Handle.State)
            {
                case CoroutineState.Running:
                    return MoveCoroutine();

                case CoroutineState.Pausing:
                    return true;

                case CoroutineState.Finished:
                    return false;

                default:
                    Handle.State = CoroutineState.Running;
                    goto case CoroutineState.Running;
            }
        }

        public void Dispose()
        {
            (coroutine as IDisposable)?.Dispose();
        }
    }
}