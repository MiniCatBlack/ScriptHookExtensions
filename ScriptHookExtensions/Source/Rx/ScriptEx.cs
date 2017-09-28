using System;
using System.Windows.Forms;
using UniRx;

namespace GTA.Extensions
{
    /// <summary>
    /// Supports Rx and Coroutine based scripting.
    /// </summary>
    public abstract class ScriptEx : Script
    {
        private readonly Subject<Unit> preUpdateSubject = new Subject<Unit>();

        private readonly Subject<Unit> updateSubject = new Subject<Unit>();

        /// <summary>
        /// Occurs before <see cref="Update"/> event occurs.
        /// </summary>
        public event EventHandler PreUpdate;

        /// <summary>
        /// Occurs once per frame.
        /// </summary>
        public event EventHandler Update;

        /// <summary>
        /// Gets the observable stream that notifies when <see cref="Script.Aborted"/> event occurs.
        /// </summary>
        public UniRx.IObservable<Unit> AbortedAsObservable { get; }

        /// <summary>
        /// Gets the observable stream that notifies when <see cref="Script.KeyDown"/> event occurs.
        /// </summary>
        public UniRx.IObservable<KeyEventArgs> KeyDownAsObservable { get; }

        /// <summary>
        /// Gets the observable stream that notifies when <see cref="Script.KeyUp"/> event occurs.
        /// </summary>
        public UniRx.IObservable<KeyEventArgs> KeyUpAsObservable { get; }

        /// <summary>
        /// Gets the observable stream that notifies when <see cref="PreUpdate"/> event occurs.
        /// </summary>
        public UniRx.IObservable<Unit> PreUpdateAsObservable { get; }

        /// <summary>
        /// Gets the observable stream that notifies when <see cref="Update"/> event occurs.
        /// </summary>
        public UniRx.IObservable<Unit> UpdateAsObservable { get; }

        /// <summary>
        /// Gets the CompositeDisposable that disposes when this script ends.
        /// </summary>
        public CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptEx"/> class.
        /// </summary>
        protected ScriptEx()
        {
            Tick += OnTick;

            AbortedAsObservable =Observable.FromEvent<EventHandler, EventArgs>(
                onNext => (sender, eventArgs) => onNext(eventArgs),
                handler => Aborted += handler,
                handler => Aborted -= handler)
                .Select(_ => Unit.Default)
                .Publish()
                .RefCount();

            KeyDownAsObservable = Observable.FromEvent<KeyEventHandler, KeyEventArgs>(
                onNext => (sender, eventArgs) => onNext(eventArgs),
                handler => KeyDown += handler,
                handler => KeyDown -= handler)
                .Publish()
                .RefCount();

            KeyUpAsObservable = Observable.FromEvent<KeyEventHandler, KeyEventArgs>(
                onNext => (sender, eventArgs) => onNext(eventArgs),
                handler => KeyUp += handler,
                handler => KeyUp -= handler)
                .Publish()
                .RefCount();

            PreUpdateAsObservable = preUpdateSubject.AsObservable();

            UpdateAsObservable = updateSubject.AsObservable();
        }

        private void OnTick(object sender, EventArgs args)
        {
            preUpdateSubject.OnNext(Unit.Default);
            PreUpdate?.Invoke(this, EventArgs.Empty);

            updateSubject.OnNext(Unit.Default);
            Update?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Disposes all disposables the CompositeDisposable contains.
        /// </summary>
        /// <param name="disposing">An boolean value that indicates whether the method was invoked from the <see cref="Script.Dispose"/> or from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Tick -= OnTick;
            CompositeDisposable.Dispose();
        }
    }
}