using mvvm.unity.Core;
using System;
using System.Collections.Generic;

namespace mvvm.unity.Samples
{
    public class ViewsProvider<T> : IViewsProvider<T> where T : Enum
    {
        private readonly IReadOnlyDictionary<T, IView> _viewsMap;

        public ViewsProvider(IReadOnlyDictionary<T, IView> viewsMap) => _viewsMap = viewsMap;

        public IReadOnlyDictionary<T, IView> GetViews() => _viewsMap;
    }
}