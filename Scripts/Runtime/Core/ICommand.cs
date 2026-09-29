using System;

namespace MvvmUnity.Core
{
    public interface ICommand
    {
        event Action CanExecuteChanged;

        bool CanExecute { get; }

        void Execute();
    }
}
