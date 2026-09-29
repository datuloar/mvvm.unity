using System;

namespace MvvmUnity.Unity
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class GenerateBindingsAttribute : Attribute
    {
        public bool Conventions { get; set; } = true;
    }
}
