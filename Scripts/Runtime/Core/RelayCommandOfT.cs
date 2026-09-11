using System;

namespace MvvmUnity.Core
{
    public sealed class RelayCommand<T> : ICommand<T>, IDisposable
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly CompositeDisposable _triggers = new CompositeDisposable();

        public RelayCommand(Action<T> execute)
            : this(execute, Always)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public event Action CanExecuteChanged = delegate { };

        public bool CanExecute(T value)
        {
            return _canExecute(value);
        }

        public void Execute(T value)
        {
            if (_canExecute(value))
                _execute(value);
        }

        public void Refresh()
        {
            CanExecuteChanged();
        }

        /// Пересчитывает CanExecute при каждом изменении source; подписка снимается в Dispose().
        public RelayCommand<T> RefreshOn<TTrigger>(IReadOnlyObservableValue<TTrigger> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            Action<TTrigger> handler = _ => Refresh();
            source.Changed += handler;
            _triggers.Add(new ActionDisposable(() => source.Changed -= handler));
            return this;
        }

        public void Dispose()
        {
            _triggers.Dispose();
        }

        private static bool Always(T value)
        {
            return true;
        }
    }
}
