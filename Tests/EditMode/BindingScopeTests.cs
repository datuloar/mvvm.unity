using MvvmUnity.Core;
using MvvmUnity.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Tests
{
    public sealed class BindingScopeTests
    {
        [Test]
        public void ObservePushesInitialValueAndStopsAfterDispose()
        {
            var source = new ObservableValue<int>(7);
            var observed = 0;
            var scope = new BindingScope();

            scope.Observe(source, value => observed = value);
            Assert.AreEqual(7, observed);

            source.Value = 9;
            Assert.AreEqual(9, observed);

            scope.Dispose();
            source.Value = 11;
            Assert.AreEqual(9, observed);
        }
        [Test]
        public void Dispose_RemovesTwoWaySliderBinding()
        {
            var targetObject = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
            try
            {
                var target = targetObject.GetComponent<Slider>();
                var source = new ObservableValue<float>(0.25f);
                var bindings = new BindingScope();
                bindings.Slider(target, source);

                source.Value = 0.5f;
                Assert.AreEqual(0.5f, target.value);
                target.value = 0.75f;
                Assert.AreEqual(0.75f, source.Value);

                bindings.Dispose();
                source.Value = 0.2f;
                Assert.AreEqual(0.75f, target.value);
                target.value = 0.4f;
                Assert.AreEqual(0.2f, source.Value);
            }
            finally
            {
                Object.DestroyImmediate(targetObject);
            }
        }

        [Test]
        public void Dispose_RemovesColorBinding()
        {
            var targetObject = new GameObject("Image", typeof(RectTransform), typeof(Image));
            try
            {
                var target = targetObject.GetComponent<Image>();
                var source = new ObservableValue<Color>(Color.white);
                var bindings = new BindingScope();
                bindings.Color(target, source);

                source.Value = Color.red;
                Assert.AreEqual(Color.red, target.color);

                bindings.Dispose();
                source.Value = Color.blue;
                Assert.AreEqual(Color.red, target.color);
            }
            finally
            {
                Object.DestroyImmediate(targetObject);
            }
        }

        [Test]
        public void Dispose_RemovesTwoWayScrollbarBinding()
        {
            var targetObject = new GameObject("Scrollbar", typeof(RectTransform), typeof(Scrollbar));
            try
            {
                var target = targetObject.GetComponent<Scrollbar>();
                var source = new ObservableValue<float>(0.25f);
                var bindings = new BindingScope();
                bindings.Scrollbar(target, source);

                source.Value = 0.5f;
                Assert.AreEqual(0.5f, target.value);
                target.value = 0.75f;
                Assert.AreEqual(0.75f, source.Value);

                bindings.Dispose();
                source.Value = 0.2f;
                Assert.AreEqual(0.75f, target.value);
                target.value = 0.4f;
                Assert.AreEqual(0.2f, source.Value);
            }
            finally
            {
                Object.DestroyImmediate(targetObject);
            }
        }

        [Test]
        public void Dispose_RemovesTwoWayDropdownBinding()
        {
            var targetObject = new GameObject("Dropdown", typeof(RectTransform), typeof(Dropdown));
            try
            {
                var target = targetObject.GetComponent<Dropdown>();
                target.options.Add(new Dropdown.OptionData("A"));
                target.options.Add(new Dropdown.OptionData("B"));
                var source = new ObservableValue<int>(0);
                var bindings = new BindingScope();
                bindings.Dropdown(target, source);

                source.Value = 1;
                Assert.AreEqual(1, target.value);
                target.value = 0;
                Assert.AreEqual(0, source.Value);

                bindings.Dispose();
                source.Value = 1;
                Assert.AreEqual(0, target.value);
            }
            finally
            {
                Object.DestroyImmediate(targetObject);
            }
        }

        [Test]
        public void ParameterizedCommandBinding_UsesArgumentAndStopsAfterDispose()
        {
            var targetObject = new GameObject("Button", typeof(RectTransform), typeof(Button));
            try
            {
                var received = string.Empty;
                var enabled = true;
                var target = targetObject.GetComponent<Button>();
                var command = new RelayCommand<string>(value => received = value, _ => enabled);
                var bindings = new BindingScope();

                bindings.Command(target, command, "station-01");
                target.onClick.Invoke();
                Assert.AreEqual("station-01", received);

                enabled = false;
                command.Refresh();
                Assert.IsFalse(target.interactable);

                bindings.Dispose();
                enabled = true;
                command.Refresh();
                Assert.IsFalse(target.interactable);
                received = string.Empty;
                target.onClick.Invoke();
                Assert.AreEqual(string.Empty, received);
            }
            finally
            {
                Object.DestroyImmediate(targetObject);
            }
        }

        [Test]
        public void ClickBinding_CallsIntentAndStopsAfterDispose()
        {
            var targetObject = new GameObject("Button", typeof(RectTransform), typeof(Button));
            try
            {
                var executions = 0;
                var target = targetObject.GetComponent<Button>();
                var bindings = new BindingScope();

                bindings.Click(target, () => executions++);
                target.onClick.Invoke();
                bindings.Dispose();
                target.onClick.Invoke();

                Assert.AreEqual(1, executions);
            }
            finally
            {
                Object.DestroyImmediate(targetObject);
            }
        }
    }
}
