using System;

using MvvmUnity.Unity;

namespace MvvmUnity.Editor
{
    internal sealed class BindingRule
    {
        private readonly Func<Type, bool> _acceptsSource;
        private readonly Type[] _widgets;

        public BindingRule(BindingTarget target, Func<Type, bool> acceptsSource, params Type[] widgets)
        {
            Target = target;
            _acceptsSource = acceptsSource;
            _widgets = widgets;
        }

        public BindingTarget Target { get; }

        public bool Matches(Type widget, Type source) =>
            _acceptsSource(source) && Array.Exists(_widgets, candidate => candidate.IsAssignableFrom(widget));
    }
}
