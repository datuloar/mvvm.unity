using System;

namespace MvvmUnity.Core
{
    public interface ICommand<in T>
    {
        event Action CanExecuteChanged;

        bool CanExecute(T argument);

        void Execute(T argument);
    }
}
