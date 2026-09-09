using System;

namespace MvvmUnity.Core
{
    public sealed class RelayCommand<T> : ICommand<T>
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

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

        private static bool Always(T value)
        {
            return true;
        }
    }
}
