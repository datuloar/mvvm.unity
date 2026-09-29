using System.Collections.Generic;

using MvvmUnity.Core;
using MvvmUnity.Unity;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Tests
{
    public sealed class BindingExtensionsTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private BindingScope _bindings;

        [SetUp]
        public void SetUp() => _bindings = new BindingScope();

        [TearDown]
        public void TearDown()
        {
            _bindings.Dispose();
            foreach (var target in _objects)
                Object.DestroyImmediate(target);

            _objects.Clear();
        }

        [Test]
        public void TextFollowsSourceUntilDisposed()
        {
            var target = Create<Text>();
            var source = new ObservableValue<string>("Ready");

            _bindings.Text(target, source);
            source.Value = "Done";
            _bindings.Dispose();
            source.Value = "Ignored";

            Assert.That(target.text, Is.EqualTo("Done"));
        }

        [Test]
        public void FillClampsSource()
        {
            var target = Create<Image>();

            _bindings.Fill(target, new ObservableValue<float>(2f));

            Assert.That(target.fillAmount, Is.EqualTo(1f));
        }

        [Test]
        public void ColorFollowsSourceUntilDisposed()
        {
            var target = Create<Image>();
            var source = new ObservableValue<Color>(Color.white);

            _bindings.Color(target, source);
            source.Value = Color.red;
            _bindings.Dispose();
            source.Value = Color.blue;

            Assert.That(target.color, Is.EqualTo(Color.red));
        }

        [Test]
        public void ActiveTogglesGameObject()
        {
            var target = new GameObject("Badge");
            _objects.Add(target);

            _bindings.Active(target, new ObservableValue<bool>(false));

            Assert.That(target.activeSelf, Is.False);
        }

        [Test]
        public void VisibleControlsCanvasGroup()
        {
            var target = Create<CanvasGroup>();

            _bindings.Visible(target, new ObservableValue<bool>(false));

            Assert.That(target.alpha, Is.Zero);
            Assert.That(target.blocksRaycasts, Is.False);
        }

        [Test]
        public void CommandExecutesAndTracksAvailabilityUntilDisposed()
        {
            var target = Create<Button>();
            var enabled = new ObservableValue<bool>(true);
            var executions = 0;
            var command = new RelayCommand(() => executions++, () => enabled.Value).RefreshOn(enabled);

            _bindings.Command(target, command);
            target.onClick.Invoke();
            enabled.Value = false;
            Assert.That(target.interactable, Is.False);

            _bindings.Dispose();
            enabled.Value = true;
            target.onClick.Invoke();
            Assert.That(target.interactable, Is.False);
            Assert.That(executions, Is.EqualTo(1));
        }

        [Test]
        public void ParameterizedCommandUsesArgument()
        {
            var target = Create<Button>();
            var received = string.Empty;

            _bindings.Command(target, new RelayCommand<string>(value => received = value), "station-01");
            target.onClick.Invoke();

            Assert.That(received, Is.EqualTo("station-01"));
        }

        [Test]
        public void ClickCallsIntentUntilDisposed()
        {
            var target = Create<Button>();
            var executions = 0;

            _bindings.Click(target, () => executions++);
            target.onClick.Invoke();
            _bindings.Dispose();
            target.onClick.Invoke();

            Assert.That(executions, Is.EqualTo(1));
        }

        [Test]
        public void SliderIsTwoWayUntilDisposed()
        {
            var target = Create<Slider>();
            var source = new ObservableValue<float>(0.25f);

            _bindings.Slider(target, source);
            source.Value = 0.5f;
            Assert.That(target.value, Is.EqualTo(0.5f));
            target.value = 0.75f;
            Assert.That(source.Value, Is.EqualTo(0.75f));

            _bindings.Dispose();
            target.value = 0.1f;
            Assert.That(source.Value, Is.EqualTo(0.75f));
        }

        [Test]
        public void ScrollbarIsTwoWay()
        {
            var target = Create<Scrollbar>();
            var source = new ObservableValue<float>(0.25f);

            _bindings.Scrollbar(target, source);
            target.value = 0.75f;

            Assert.That(source.Value, Is.EqualTo(0.75f));
        }

        [Test]
        public void ToggleIsTwoWay()
        {
            var target = Create<Toggle>();
            var source = new ObservableValue<bool>(true);

            _bindings.Toggle(target, source);
            target.isOn = false;

            Assert.That(source.Value, Is.False);
        }

        [Test]
        public void InputIsTwoWay()
        {
            var target = Create<TMP_InputField>();
            var source = new ObservableValue<string>("Ann");

            _bindings.Input(target, source);
            Assert.That(target.text, Is.EqualTo("Ann"));
            target.text = "Bob";

            Assert.That(source.Value, Is.EqualTo("Bob"));
        }

        [Test]
        public void DropdownIsTwoWay()
        {
            var target = Create<TMP_Dropdown>();
            target.AddOptions(new List<string> { "A", "B" });
            var source = new ObservableValue<int>(1);

            _bindings.Dropdown(target, source);
            Assert.That(target.value, Is.EqualTo(1));
            target.value = 0;

            Assert.That(source.Value, Is.Zero);
        }

        private T Create<T>()
            where T : Component
        {
            var target = new GameObject(typeof(T).Name, typeof(RectTransform), typeof(T));
            _objects.Add(target);
            return target.GetComponent<T>();
        }
    }
}
