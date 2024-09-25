using System;
using System.Collections.Generic;
using System.Reflection;

namespace mvvm.unity.Core
{
    public class Property : IProperty
    {
        private readonly ViewModel _target;
        private readonly PropertyInfo _propertyInfo;

        public Property(ViewModel target, PropertyInfo propertyInfo, string name)
        {
            _target = target;
            _propertyInfo = propertyInfo;
            Name = name;
        }

        public string Name { get; }

        public event Action<IProperty> Changed = delegate { };

        public object Get() => _propertyInfo.GetValue(_target);

        public void Set(object value)
        {
            if (value == null)
                throw new ArgumentNullException($"Failed to assign value to property {Name}, argument is null");

            var currentValue = Get();

            if (Comparer<object>.Default.Compare(currentValue, value) != 0)
                _propertyInfo.SetValue(_target, value);
        }

        public void OnChanged() => Changed.Invoke(this);
    }
}