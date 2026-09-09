namespace MvvmUnity.Core
{
    public interface IObservableValue<T> : IReadOnlyObservableValue<T>
    {
        new T Value { get; set; }

        bool Set(T value);

        /// Оповестить подписчиков текущим значением. Нужно, когда состояние —
        /// изменяемый объект: Set сравнивает по Equals и на том же экземпляре молчит.
        void Refresh();
    }
}
