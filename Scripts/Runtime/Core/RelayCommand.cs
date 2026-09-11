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

        /// Пересчитывает CanExecute при каждом изменении source; подписка снимается в Dispose().
        public RelayCommand RefreshOn<T>(IReadOnlyObservableValue<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            Action<T> handler = _ => Refresh();
            source.Changed += handler;
            _triggers.Add(new ActionDisposable(() => source.Changed -= handler));
            return this;
        }

        public void Dispose()
        {
            _triggers.Dispose();
        }

        private static bool Always()
        {
            return true;
        }
    }
}
