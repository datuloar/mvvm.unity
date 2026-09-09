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
            var action = _dispose;
            _dispose = null;
            if (action != null)
                action();
        }
    }
}
