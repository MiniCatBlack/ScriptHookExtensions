using System;

namespace GTA.Extensions
{
    internal class PrimaryCoroutine : CoroutineHandle
    {
        public override CoroutineState State { get; internal set; }

        public override void Pause()
        {
            if (State == CoroutineState.Running)
                State = CoroutineState.Pausing;
        }

        public override void Resume()
        {
            if (State == CoroutineState.Pausing)
                State = CoroutineState.Running;
        }

        public override void Kill()
        {
            State = CoroutineState.Finished;
        }
    }
}