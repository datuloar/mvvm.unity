using System;
using System.Collections.Generic;

namespace MvvmUnity.Core
{
    public sealed class ObservableValue<T> : IObservableValue<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private T _value;

        public ObservableValue()
            : this(default)
        {
        }

        public ObservableValue(T value)
            : this(value, EqualityComparer<T>.Default)
        {
        }

        public ObservableValue(T value, IEqualityComparer<T> comparer)
        {
            _value = value;
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public event Action<T> Changed;

        public T Value
        {
            get => _value;
            set => Set(value);
        }

        public bool Set(T value)
        {
            if (_comparer.Equals(_value, value))
                return false;

            _value = value;
            Changed?.Invoke(value);
            return true;
        }

        public void Refresh() => Changed?.Invoke(_value);
    }
}
