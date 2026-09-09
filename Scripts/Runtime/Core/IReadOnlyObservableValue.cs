using System;

namespace MvvmUnity.Core
{
    public interface IReadOnlyObservableValue<T>
    {
        T Value { get; }

        event Action<T> Changed;
    }
}
