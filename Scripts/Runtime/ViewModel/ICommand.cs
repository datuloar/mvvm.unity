namespace mvvm.unity.Core
{
    public interface ICommand : IBindable
    {
        void Execute();
    }
}