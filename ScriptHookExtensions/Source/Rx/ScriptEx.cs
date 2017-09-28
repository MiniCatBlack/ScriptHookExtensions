using System;

namespace GTA.Extensions
{
    /// <summary>
    /// Supports Rx and Coroutine based scripting.
    /// </summary>
    public abstract class ScriptEx : Script
    {
        /// <summary>
        /// Occurs before <see cref="Update"/> event occurs.
        /// </summary>
        public event EventHandler PreUpdate;

        /// <summary>
        /// Occurs once per frame.
        /// </summary>
        public event EventHandler Update;

        /// <summary>
        /// Initializes a new instance of the ScriptEx class.
        /// </summary>
        protected ScriptEx()
        {

        }

        private void OnTick(object sender, EventArgs args)
        {
            PreUpdate?.Invoke(this, EventArgs.Empty);

            Update?.Invoke(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Tick -= OnTick;
        }
    }
}