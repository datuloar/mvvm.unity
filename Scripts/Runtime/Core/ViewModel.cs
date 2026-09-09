namespace MvvmUnity.Core
{
    public abstract class ViewModel : IViewModel
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}
