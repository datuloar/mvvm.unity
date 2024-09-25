using System;

namespace mvvm.unity.Core
{
    public interface IProperty : IBindable
    {
        event Action<IProperty> Changed;

        object Get();
        void Set(object value);
    }
}