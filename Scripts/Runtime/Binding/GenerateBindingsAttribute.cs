using System;

namespace MvvmUnity.Unity
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class GenerateBindingsAttribute : Attribute
    {
        public GenerateBindingsAttribute()
        {
        }

        public GenerateBindingsAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
        }

        public Type ViewModelType { get; }

        public bool Conventions { get; set; } = true;
    }
}
