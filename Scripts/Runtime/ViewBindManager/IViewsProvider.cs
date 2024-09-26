using System;
using System.Collections.Generic;

namespace mvvm.unity.Core
{
    public interface IViewsProvider<T> where T : Enum
    {
        IReadOnlyDictionary<T, IView> GetViews();
    }
}