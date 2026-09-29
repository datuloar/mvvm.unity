using System;
using System.Collections.Generic;

namespace MvvmUnity.Editor
{
    internal sealed class ViewBindingPlan
    {
        public ViewBindingPlan(
            Type viewType,
            Type viewModelType,
            IReadOnlyList<FieldBinding> fields,
            IReadOnlyList<ObserverBinding> observers)
        {
            ViewType = viewType;
            ViewModelType = viewModelType;
            Fields = fields;
            Observers = observers;
        }

        public Type ViewType { get; }

        public Type ViewModelType { get; }

        public IReadOnlyList<FieldBinding> Fields { get; }

        public IReadOnlyList<ObserverBinding> Observers { get; }
    }
}
