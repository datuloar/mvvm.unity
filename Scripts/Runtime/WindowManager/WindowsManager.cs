using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace mvvm.unity.Core
{
    public class WindowsManager<T> where T : Enum
    {
        private readonly IWindowsProvider<T> _windowsProvider;
        private readonly Dictionary<IWindow, Binder> _windowBinders = new();

        public WindowsManager(IWindowsProvider<T> windowsProvider)
        {
            _windowsProvider = windowsProvider;
        }

        public async Task BindAndShowAsync<TModel>(T windowType, TModel model) where TModel : ViewModel
        {
            var windows = _windowsProvider.GetWindows();

            if (windows.TryGetValue(windowType, out var window))
            {
                if (window is View view)
                {
                    view.Initialize();

                    var binder = new Binder(view, model);

                    binder.Bind();
                    _windowBinders[window] = binder;

                    await window.ShowAsync();
                }
                else
                {
                    Debug.LogError($"Window of type {windowType} is not a valid View.");
                }
            }
            else
            {
                Debug.LogError($"Window of type {windowType} not found.");
            }
        }

        public async Task HideAndUnbindAsync(T windowType)
        {
            var windows = _windowsProvider.GetWindows();

            if (windows.TryGetValue(windowType, out var window))
            {
                await window.HideAsync();

                if (_windowBinders.TryGetValue(window, out var binder))
                {
                    binder.Unbind();
                    _windowBinders.Remove(window);
                }
            }
            else
            {
                Debug.LogError($"Window of type {windowType} not found.");
            }
        }
    }
}