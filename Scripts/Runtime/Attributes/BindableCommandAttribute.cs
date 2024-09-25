using System;

namespace mvvm.unity.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BindableCommandAttribute : Attribute
    {
        public BindableCommandAttribute(string key = null)
        {
            Key = key;
        }

        public string Key { get; }
    }
}