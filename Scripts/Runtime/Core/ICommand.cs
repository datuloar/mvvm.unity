using System;

namespace MvvmUnity.Core
{
    public interface ICommand
    {
        bool CanExecute { get; }

        event Action CanExecuteChanged;

        void Execute();
    }
}
