using System;

namespace MvvmUnity.Core
{
    public abstract class ViewModel : IViewModel
    {
        private readonly CompositeDisposable _ownedResources = new CompositeDisposable();
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            OnDispose();
            _ownedResources.Dispose();
        }

        protected TResource Own<TResource>(TResource resource)
            where TResource : IDisposable
        {
            _ownedResources.Add(resource);
            return resource;
        }

        protected virtual void OnDispose()
        {
        }
    }
}
