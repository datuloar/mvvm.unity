using System;
using System.Collections.Generic;

namespace MvvmUnity.Core
{
    public sealed class ObservableValue<T> : IObservableValue<T>
    {
        private readonly IEqualityComparer<T> _equality;
        private T _value;

        public ObservableValue(T value)
            : this(value, EqualityComparer<T>.Default)
        {
        }

        public ObservableValue(T value, IEqualityComparer<T> equality)
        {
            _value = value;
            _equality = equality ?? throw new ArgumentNullException(nameof(equality));
        }

        public T Value
        {
            get => _value;
            set => Set(value);
        }

        public event Action<T> Changed = delegate { };

        public bool Set(T value)
        {
            if (_equality.Equals(_value, value))
                return false;
            _value = value;
            Changed(value);
            return true;
        }

        public void Refresh()
        {
            Changed(_value);
        }
    }
}
