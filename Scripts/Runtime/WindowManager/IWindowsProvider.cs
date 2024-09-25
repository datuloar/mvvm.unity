using System;
using System.Collections.Generic;

namespace mvvm.unity.Core

{
    public interface IWindowsProvider<T> where T : Enum
    {
        IReadOnlyDictionary<T, IWindow> GetWindows();
    }
}