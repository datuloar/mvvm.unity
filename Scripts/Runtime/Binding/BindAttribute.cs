using System;

namespace MvvmUnity.Unity
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class BindAttribute : Attribute
    {
        public BindAttribute(string source)
            : this(source, BindingTarget.Auto)
        {
        }

        public BindAttribute(string source, BindingTarget target)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target;
        }

        public string Source { get; }
        public BindingTarget Target { get; }
    }
}
