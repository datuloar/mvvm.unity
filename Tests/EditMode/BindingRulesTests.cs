using System;

using MvvmUnity.Core;
using MvvmUnity.Editor;
using MvvmUnity.Unity;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Tests
{
    public sealed class BindingRulesTests
    {
        [TestCase(typeof(Button), typeof(ICommand), BindingTarget.Command)]
        [TestCase(typeof(Button), typeof(Action), BindingTarget.Click)]
        [TestCase(typeof(Button), typeof(IReadOnlyObservableValue<bool>), BindingTarget.Interactable)]
        [TestCase(typeof(TMP_Text), typeof(IReadOnlyObservableValue<string>), BindingTarget.Text)]
        [TestCase(typeof(Image), typeof(IReadOnlyObservableValue<Sprite>), BindingTarget.Sprite)]
        [TestCase(typeof(Image), typeof(IReadOnlyObservableValue<float>), BindingTarget.Fill)]
        [TestCase(typeof(RawImage), typeof(IReadOnlyObservableValue<Color>), BindingTarget.Color)]
        [TestCase(typeof(GameObject), typeof(IReadOnlyObservableValue<bool>), BindingTarget.Active)]
        [TestCase(typeof(CanvasGroup), typeof(IReadOnlyObservableValue<bool>), BindingTarget.Visible)]
        [TestCase(typeof(Slider), typeof(IObservableValue<float>), BindingTarget.Slider)]
        [TestCase(typeof(Scrollbar), typeof(ObservableValue<float>), BindingTarget.Scrollbar)]
        [TestCase(typeof(Toggle), typeof(IObservableValue<bool>), BindingTarget.Toggle)]
        [TestCase(typeof(TMP_InputField), typeof(IObservableValue<string>), BindingTarget.Input)]
        [TestCase(typeof(TMP_Dropdown), typeof(IObservableValue<int>), BindingTarget.Dropdown)]
        public void InfersTheDocumentedTarget(Type widget, Type source, BindingTarget expected)
        {
            Assert.That(BindingRules.TryInfer(widget, source, out var target), Is.True);
            Assert.That(target, Is.EqualTo(expected));
        }

        [TestCase(typeof(Slider), typeof(IReadOnlyObservableValue<float>))]
        [TestCase(typeof(TMP_Text), typeof(IReadOnlyObservableValue<int>))]
        [TestCase(typeof(Image), typeof(ICommand))]
        public void RejectsIncompatiblePairs(Type widget, Type source)
        {
            Assert.That(BindingRules.TryInfer(widget, source, out _), Is.False);
        }

        [Test]
        public void ExplicitTargetMustBeCompatible()
        {
            Assert.That(BindingRules.Supports(BindingTarget.Interactable, typeof(Toggle), typeof(IObservableValue<bool>)), Is.True);
            Assert.That(BindingRules.Supports(BindingTarget.Fill, typeof(Text), typeof(IReadOnlyObservableValue<float>)), Is.False);
        }
    }
}
