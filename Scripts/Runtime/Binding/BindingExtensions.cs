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
        public static void Text(
            this BindingScope scope,
            UnityEngine.UI.Text target,
            IReadOnlyObservableValue<string> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.text = value));
        }

        public static void Text(
            this BindingScope scope,
            TMP_Text target,
            IReadOnlyObservableValue<string> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.text = value));
        }

        public static void Sprite(
            this BindingScope scope,
            Image target,
            IReadOnlyObservableValue<Sprite> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.sprite = value));
        }

        public static void Active(
            this BindingScope scope,
            GameObject target,
            IReadOnlyObservableValue<bool> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(target.SetActive));
        }

        public static void Visible(
            this BindingScope scope,
            CanvasGroup target,
            IReadOnlyObservableValue<bool> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value =>
            {
                target.alpha = value ? 1f : 0f;
                target.interactable = value;
                target.blocksRaycasts = value;
            }));
        }

        public static void Fill(
            this BindingScope scope,
            Image target,
            IReadOnlyObservableValue<float> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.fillAmount = Mathf.Clamp01(value)));
        }

        public static void Interactable(
            this BindingScope scope,
            Selectable target,
            IReadOnlyObservableValue<bool> source)
        {
            Require(scope, target, source);
            scope.Add(source.Subscribe(value => target.interactable = value));
        }

        public static void Command(this BindingScope scope, Button target, ICommand command)
        {
            Require(scope, target, command);
            UnityAction execute = command.Execute;
            Action refresh = () => target.interactable = command.CanExecute;
            target.onClick.AddListener(execute);
            command.CanExecuteChanged += refresh;
            refresh();
            scope.Add(new ActionDisposable(() =>
            {
                target.onClick.RemoveListener(execute);
                command.CanExecuteChanged -= refresh;
            }));
        }

        public static void Click(this BindingScope scope, Button target, Action intent)
        {
            Require(scope, target, intent);
            UnityAction execute = intent.Invoke;
            target.onClick.AddListener(execute);
            scope.Add(new ActionDisposable(() => target.onClick.RemoveListener(execute)));
        }

        public static void Command<T>(
            this BindingScope scope,
            Button target,
            ICommand<T> command,
            T argument)
        {
            Require(scope, target, command);
            UnityAction execute = () => command.Execute(argument);
            Action refresh = () => target.interactable = command.CanExecute(argument);
            target.onClick.AddListener(execute);
            command.CanExecuteChanged += refresh;
            refresh();
            scope.Add(new ActionDisposable(() =>
            {
                target.onClick.RemoveListener(execute);
                command.CanExecuteChanged -= refresh;
            }));
        }

        public static void Slider(
            this BindingScope scope,
            UnityEngine.UI.Slider target,
            IObservableValue<float> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetValueWithoutNotify);
        }

        public static void Toggle(
            this BindingScope scope,
            UnityEngine.UI.Toggle target,
            IObservableValue<bool> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetIsOnWithoutNotify);
        }

        public static void Input(
            this BindingScope scope,
            InputField target,
            IObservableValue<string> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetTextWithoutNotify);
        }

        public static void Input(
            this BindingScope scope,
            TMP_InputField target,
            IObservableValue<string> source)
        {
            Require(scope, target, source);
            TwoWay(scope, source, target.onValueChanged, target.SetTextWithoutNotify);
        }

        /// Двусторонняя связь без флага «сейчас обновляемся»: запись в вид идёт через
        /// Set*WithoutNotify, поэтому обратный вызов вида не может сработать повторно.
        private static void TwoWay<T>(
            BindingScope scope,
            IObservableValue<T> source,
            UnityEvent<T> viewChanged,
            UnityAction<T> pushToView)
        {
            UnityAction<T> fromView = value => source.Value = value;
            Action<T> fromViewModel = value => pushToView(value);
            viewChanged.AddListener(fromView);
            source.Changed += fromViewModel;
            fromViewModel(source.Value);
            scope.Add(new ActionDisposable(() =>
            {
                viewChanged.RemoveListener(fromView);
                source.Changed -= fromViewModel;
            }));
        }

        private static void Require(object scope, UnityEngine.Object target, object source)
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
