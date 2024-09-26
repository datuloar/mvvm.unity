using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

namespace mvvm.unity.Core
{
    public class Binder
    {
        private readonly IView _view;
        private readonly IViewModel _viewModel;

        public Binder(IView view, IViewModel viewModel)
        {
            _view = view;
            _viewModel = viewModel;
        }

        public void Bind()
        {
            var commands = _viewModel.GetCommands().ToDictionary(cmd => cmd.Name);
            var properties = _viewModel.GetProperties().ToDictionary(prop => prop.Name);

            BindButtons(commands);
            BindSliders(_view.Sliders, properties);
            BindProperty(_view.Texts, OnUpdateTextBinding, properties);
            BindProperty(_view.Images, OnUpdateImageBinding, properties);
            BindProperty(_view.GameObjects, OnUpdateGameObjectBinding, properties);
        }

        private void BindButtons(Dictionary<string, ICommand> commands)
        {
            foreach (var button in _view.Buttons)
            {
                if (commands.TryGetValue(button.Key, out var command))
                    button.Value.onClick.AddListener(() => command.Execute());
            }
        }

        private void BindProperty<T>(
            IReadOnlyDictionary<string, T> viewElements,
            Action<T, IProperty> updateMethod,
            Dictionary<string, IProperty> properties)
        {
            foreach (var element in viewElements)
            {
                if (properties.TryGetValue(element.Key, out var property))
                {
                    updateMethod(element.Value, property);
                    property.Changed += _ => updateMethod(element.Value, property);
                }
            }
        }

        private void BindSliders(
            IReadOnlyDictionary<string, Slider> sliders,
            Dictionary<string, IProperty> properties)
        {
            foreach (var sliderPair in sliders)
            {
                if (properties.TryGetValue(sliderPair.Key, out var property))
                {
                    var slider = sliderPair.Value;
                    OnUpdateSliderBinding(slider, property);

                    slider.onValueChanged.AddListener(value => property.Set(value));

                    property.Changed += _ => OnUpdateSliderBinding(slider, property);
                }
            }
        }

        private void OnUpdateTextBinding(TMP_Text text, IProperty property)
        {
            if (property.Get() is string newValue)
                text.text = newValue;
        }

        private void OnUpdateImageBinding(Image image, IProperty property)
        {
            if (property.Get() is Sprite newValue)
                image.sprite = newValue;
        }

        private void OnUpdateSliderBinding(Slider slider, IProperty property)
        {
            if (property.Get() is float newValue)
                slider.value = newValue;
        }

        private void OnUpdateGameObjectBinding(GameObject go, IProperty property)
        {
            if (property.Get() is bool isActive)
                go.SetActive(isActive);
        }
    }
}