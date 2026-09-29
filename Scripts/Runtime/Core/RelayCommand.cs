using System;

namespace MvvmUnity.Core
{
    public sealed class RelayCommand : ICommand, IDisposable
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        private readonly CompositeDisposable _triggers = new CompositeDisposable();

        public RelayCommand(Action execute)
            : this(execute, Always)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public event Action CanExecuteChanged;

        public bool CanExecute => _canExecute();

        public void Execute()
        {
            if (CanExecute)
                _execute();
        }

        public void Refresh() => CanExecuteChanged?.Invoke();

        public RelayCommand RefreshOn<TTrigger>(IReadOnlyObservableValue<TTrigger> trigger)
        {
            _triggers.Add(trigger.Subscribe(_ => Refresh(), false));
            return this;
        }

        public void Dispose() => _triggers.Dispose();

        private static bool Always() => true;
    }
}
