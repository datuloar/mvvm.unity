using MvvmUnity.Unity;

namespace MvvmUnity.Editor
{
    internal readonly struct FieldBinding
    {
        public FieldBinding(string field, string source, BindingTarget target)
        {
            Field = field;
            Source = source;
            Target = target;
        }

        public string Field { get; }

        public string Source { get; }

        public BindingTarget Target { get; }
    }
}
