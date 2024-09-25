using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace mvvm.unity.Core
{
    public class ViewsManager<T> where T : Enum
    {
        private readonly IReadOnlyDictionary<T, IView> _views;
        private readonly Dictionary<IView, Binder> _boundViews = new();

        public ViewsManager(IViewsProvider<T> viewsProvider)
        {
            _views = viewsProvider.GetViews();
        }

        public async Task BindAndShowAsync<TModel>(T windowType, TModel model) where TModel : IViewModel
        {
            if (_views.TryGetValue(windowType, out var window))
            {
                if (window is IView view)
                {
                    view.Initialize();

                    var binder = new Binder(view, model);

                    binder.Bind();
                    _boundViews[window] = binder;

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
            if (_views.TryGetValue(windowType, out var window))
            {
                await window.HideAsync();

                if (_boundViews.TryGetValue(window, out var binder))
                {
                    binder.Unbind();
                    _boundViews.Remove(window);
                }
            }
            else
            {
                Debug.LogError($"Window of type {windowType} not found.");
            }
        }
    }
}