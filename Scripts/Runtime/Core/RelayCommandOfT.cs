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

        public event Action CanExecuteChanged;

        public bool CanExecute(T argument) => _canExecute(argument);

        public void Execute(T argument)
        {
            if (_canExecute(argument))
                _execute(argument);
        }

        public void Refresh() => CanExecuteChanged?.Invoke();

        public RelayCommand<T> RefreshOn<TTrigger>(IReadOnlyObservableValue<TTrigger> trigger)
        {
            _triggers.Add(trigger.Subscribe(_ => Refresh(), false));
            return this;
        }

        public void Dispose() => _triggers.Dispose();

        private static bool Always(T argument) => true;
    }
}
