using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace mvvm.unity.Core
{
    public class ViewBindManager<T> where T : Enum
    {
        private readonly IReadOnlyDictionary<T, IView> _views;

        public ViewBindManager(IViewsProvider<T> viewsProvider)
        {
            _views = viewsProvider.GetViews();
        }

        public async Task BindAndShowAsync<TModel>(T viewType, TModel model) where TModel : IViewModel
        {
            if (_views.TryGetValue(viewType, out var view))
            {
                view.Initialize();

                var binder = new Binder(view, model);
                binder.Bind();

                await view.ShowAsync();
            }
            else
            {
                Debug.LogError($"View of type {viewType} not found.");
            }
        }

        public async Task HideAndUnbindAsync(T type)
        {
            if (!_views.TryGetValue(type, out var view))
            {
                Debug.LogError($"View of type {type} not found.");
                return;
            }

            await view.HideAsync();
        }
    }
}