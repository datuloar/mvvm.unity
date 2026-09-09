using System;
using MvvmUnity.Core;

namespace MvvmUnity.Unity
{
    public sealed class BindingScope : IDisposable
    {
        private readonly CompositeDisposable _bindings = new CompositeDisposable();

        public void Add(IDisposable binding)
        {
            _bindings.Add(binding);
        }

        public void Observe<T>(IReadOnlyObservableValue<T> source, Action<T> render)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (render == null)
                throw new ArgumentNullException(nameof(render));
            Add(source.Subscribe(render));
        }

        public void Dispose()
        {
            _bindings.Dispose();
        }
    }
}
