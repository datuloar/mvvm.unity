using System;

namespace MvvmUnity.Core
{
    public sealed class ActionDisposable : IDisposable
    {
        private Action _dispose;

        public ActionDisposable(Action dispose)
        {
            _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
        }

        public void Dispose()
        {
            var dispose = _dispose;
            _dispose = null;
            dispose?.Invoke();
        }
    }
}
