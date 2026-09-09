using System;

namespace MvvmUnity.Unity
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class IgnoreBindingAttribute : Attribute
    {
    }
}
