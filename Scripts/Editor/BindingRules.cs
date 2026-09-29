using System;

using MvvmUnity.Core;
using MvvmUnity.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Editor
{
    internal static class BindingRules
    {
        private static readonly BindingRule[] Rules =
        {
            new BindingRule(BindingTarget.Command, source => typeof(ICommand).IsAssignableFrom(source), typeof(Button)),
            new BindingRule(BindingTarget.Click, source => source == typeof(Action), typeof(Button)),
            new BindingRule(BindingTarget.Text, Publishes<string>, typeof(Text), typeof(TMP_Text)),
            new BindingRule(BindingTarget.Sprite, Publishes<Sprite>, typeof(Image)),
            new BindingRule(BindingTarget.Fill, Publishes<float>, typeof(Image)),
            new BindingRule(BindingTarget.Color, Publishes<Color>, typeof(Graphic)),
            new BindingRule(BindingTarget.Active, Publishes<bool>, typeof(GameObject)),
            new BindingRule(BindingTarget.Visible, Publishes<bool>, typeof(CanvasGroup)),
            new BindingRule(BindingTarget.Slider, Accepts<float>, typeof(Slider)),
            new BindingRule(BindingTarget.Scrollbar, Accepts<float>, typeof(Scrollbar)),
            new BindingRule(BindingTarget.Toggle, Accepts<bool>, typeof(Toggle)),
            new BindingRule(BindingTarget.Input, Accepts<string>, typeof(InputField), typeof(TMP_InputField)),
            new BindingRule(BindingTarget.Dropdown, Accepts<int>, typeof(Dropdown), typeof(TMP_Dropdown)),
            new BindingRule(BindingTarget.Interactable, Publishes<bool>, typeof(Selectable))
        };

        public static bool TryInfer(Type widget, Type source, out BindingTarget target)
        {
            foreach (var rule in Rules)
            {
                if (!rule.Matches(widget, source))
                    continue;

                target = rule.Target;
                return true;
            }

            target = BindingTarget.Auto;
            return false;
        }

        public static bool Supports(BindingTarget target, Type widget, Type source) =>
            Array.Exists(Rules, rule => rule.Target == target && rule.Matches(widget, source));

        private static bool Publishes<TValue>(Type source) => ObservableContracts.Publishes(source, typeof(TValue));

        private static bool Accepts<TValue>(Type source) => ObservableContracts.Accepts(source, typeof(TValue));
    }
}
