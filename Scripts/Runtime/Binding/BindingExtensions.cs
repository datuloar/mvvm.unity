using System;

using MvvmUnity.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MvvmUnity.Unity
{
    public static class BindingExtensions
    {
        public static void Text(this BindingScope scope, Text target, IReadOnlyObservableValue<string> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.text = value));
        }

        public static void Text(this BindingScope scope, TMP_Text target, IReadOnlyObservableValue<string> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.text = value));
        }

        public static void Sprite(this BindingScope scope, Image target, IReadOnlyObservableValue<Sprite> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.sprite = value));
        }

        public static void Fill(this BindingScope scope, Image target, IReadOnlyObservableValue<float> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.fillAmount = Mathf.Clamp01(value)));
        }

        public static void Color(this BindingScope scope, Graphic target, IReadOnlyObservableValue<Color> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.color = value));
        }

        public static void Active(this BindingScope scope, GameObject target, IReadOnlyObservableValue<bool> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(target.SetActive));
        }

        public static void Visible(this BindingScope scope, CanvasGroup target, IReadOnlyObservableValue<bool> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value =>
            {
                target.alpha = value ? 1f : 0f;
                target.interactable = value;
                target.blocksRaycasts = value;
            }));
        }

        public static void Interactable(this BindingScope scope, Selectable target, IReadOnlyObservableValue<bool> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.interactable = value));
        }

        public static void Command(this BindingScope scope, Button target, ICommand command)
        {
            Require(scope, target, command);
            ButtonCommand(
                scope,
                target,
                command.Execute,
                () => command.CanExecute,
                handler => command.CanExecuteChanged += handler,
                handler => command.CanExecuteChanged -= handler);
        }

        public static void Command<T>(this BindingScope scope, Button target, ICommand<T> command, T argument)
        {
            Require(scope, target, command);
            ButtonCommand(
                scope,
                target,
                () => command.Execute(argument),
                () => command.CanExecute(argument),
                handler => command.CanExecuteChanged += handler,
                handler => command.CanExecuteChanged -= handler);
        }

        public static void Click(this BindingScope scope, Button target, Action intent)
        {
            Require(scope, target, intent);
            UnityAction click = intent.Invoke;
            target.onClick.AddListener(click);
            scope.Add(new ActionDisposable(() => target.onClick.RemoveListener(click)));
        }

        public static void Slider(this BindingScope scope, Slider target, IObservableValue<float> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetValueWithoutNotify);
        }

        public static void Scrollbar(this BindingScope scope, Scrollbar target, IObservableValue<float> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetValueWithoutNotify);
        }

        public static void Toggle(this BindingScope scope, Toggle target, IObservableValue<bool> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetIsOnWithoutNotify);
        }

        public static void Input(this BindingScope scope, InputField target, IObservableValue<string> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetTextWithoutNotify);
        }

        public static void Input(this BindingScope scope, TMP_InputField target, IObservableValue<string> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetTextWithoutNotify);
        }

        public static void Dropdown(this BindingScope scope, Dropdown target, IObservableValue<int> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetValueWithoutNotify);
        }

        public static void Dropdown(this BindingScope scope, TMP_Dropdown target, IObservableValue<int> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetValueWithoutNotify);
        }

        private static void ButtonCommand(
            BindingScope scope,
            Button target,
            Action execute,
            Func<bool> canExecute,
            Action<Action> subscribe,
            Action<Action> unsubscribe)
        {
            UnityAction click = execute.Invoke;
            Action refresh = () => target.interactable = canExecute();
            target.onClick.AddListener(click);
            subscribe(refresh);
            refresh();
            scope.Add(new ActionDisposable(() =>
            {
                target.onClick.RemoveListener(click);
                unsubscribe(refresh);
            }));
        }

        private static void TwoWay<T>(
            BindingScope scope,
            IObservableValue<T> source,
            UnityEvent<T> viewChanged,
            UnityAction<T> pushToView)
        {
            UnityAction<T> fromView = value => source.Value = value;
            Action<T> fromViewModel = pushToView.Invoke;
            viewChanged.AddListener(fromView);
            source.Changed += fromViewModel;
            fromViewModel(source.Value);
            scope.Add(new ActionDisposable(() =>
            {
                viewChanged.RemoveListener(fromView);
                source.Changed -= fromViewModel;
            }));
        }

        private static void Require(BindingScope scope, UnityEngine.Object target, object source)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
        }
    }
}
