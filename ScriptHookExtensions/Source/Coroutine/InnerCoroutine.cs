using System;
using System.Collections;

namespace GTA.Extensions
{
    internal class InnerCoroutine : CoroutineHandle
    {
        private readonly PrimaryCoroutine primary;

        internal IEnumerator Enumerator { get; }

        internal InnerCoroutine(IEnumerator enumerator, PrimaryCoroutine primary)
        {
            this.primary = primary;
            Enumerator = enumerator;
        }

        public override CoroutineState State { get => primary.State; internal set => primary.State = value; }

        public override void Pause() => primary.Pause();

        public override void Resume() => primary.Resume();

        public override void Kill() => primary.Kill();
    }
}