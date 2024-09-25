using System;

namespace mvvm.unity.Core
{
    public abstract class BindableDataAttribute : Attribute
    {
        public BindableDataAttribute(string key = null)
        {
            Key = key;
        }

        public string Key { get; }
    }
}