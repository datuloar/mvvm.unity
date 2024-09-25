using System;

namespace mvvm.unity.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindablePropertyAttribute : Attribute
    {
        public BindablePropertyAttribute(string key = null)
        {
            Key = key;
        }

        public string Key { get; }
    }
}