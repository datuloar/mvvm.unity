using System;

namespace MvvmUnity.Core
{
    public interface ICommand<T>
    {
        event Action CanExecuteChanged;

        bool CanExecute(T value);

        void Execute(T value);
    }
}
