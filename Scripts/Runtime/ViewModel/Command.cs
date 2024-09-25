using System.Reflection;

namespace mvvm.unity.Core
{
    public class Command : ICommand
    {
        private readonly ViewModel _target;
        private readonly MethodInfo _method;
        private readonly object[] _emptyArguments = new object[0];

        public Command(ViewModel target, MethodInfo methodInfo, string name)
        {
            _target = target;
            _method = methodInfo;
            Name = name;
        }

        public string Name { get; }

        public void Execute() => _method.Invoke(_target, _emptyArguments);
    }
}