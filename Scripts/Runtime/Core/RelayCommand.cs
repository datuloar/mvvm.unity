using System;

namespace MvvmUnity.Core
{
    public sealed class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute)
            : this(execute, Always)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public bool CanExecute => _canExecute();

        public event Action CanExecuteChanged = delegate { };

        public void Execute()
        {
            if (CanExecute)
                _execute();
        }

        public void Refresh()
        {
            CanExecuteChanged();
        }

        private static bool Always()
        {
            return true;
        }
    }
}
