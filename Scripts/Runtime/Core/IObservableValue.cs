namespace MvvmUnity.Core
{
    public interface IObservableValue<T> : IReadOnlyObservableValue<T>
    {
        new T Value { get; set; }

        bool Set(T value);

        void Refresh();
    }
}
