using System;

namespace MvvmUnity.Unity
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ObserveAttribute : Attribute
    {
        public ObserveAttribute()
            : this(string.Empty)
        {
        }

        public ObserveAttribute(string source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public string Source { get; }
    }
}
