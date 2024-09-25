using mvvm.unity.Core;
using System;
using System.Collections.Generic;

namespace mvvm.unity.Samples
{
    public class WindowsProvider<T> : IWindowsProvider<T> where T : Enum
    {
        private readonly IReadOnlyDictionary<T, IWindow> _windowsMap;

        public WindowsProvider(IReadOnlyDictionary<T, IWindow> windowsMap) => _windowsMap = windowsMap;

        public IReadOnlyDictionary<T, IWindow> GetWindows() => _windowsMap;
    }
}