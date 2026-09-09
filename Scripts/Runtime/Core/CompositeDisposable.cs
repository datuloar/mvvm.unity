using System;
using System.Collections.Generic;

namespace MvvmUnity.Core
{
    public sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _items = new List<IDisposable>();
        private bool _disposed;

        public void Add(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (_disposed)
            {
                item.Dispose();
                return;
            }
            _items.Add(item);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            for (var index = _items.Count - 1; index >= 0; index--)
                _items[index].Dispose();
            _items.Clear();
        }
    }
}
