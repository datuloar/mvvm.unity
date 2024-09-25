using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace mvvm.unity.Core
{
    public class Binder
    {
        private readonly IView _view;
        private readonly IViewModel _viewModel;

        private readonly Dictionary<Button, ICommand> _buttonCommandMap = new();
        private readonly Dictionary<TMP_Text, IProperty> _textPropertyMap = new();
        private readonly Dictionary<Image, IProperty> _imagePropertyMap = new();
        private readonly Dictionary<Slider, IProperty> _sliderPropertyMap = new();

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
            BindTexts(properties);
            BindImages(properties);
            BindSliders(properties);
        }

        private void BindButtons(Dictionary<string, ICommand> commands)
        {
            foreach (var button in _view.Buttons)
            {
                if (commands.TryGetValue(button.Key, out var command))
                {
                    _buttonCommandMap[button.Value] = command;
                    button.Value.onClick.AddListener(() => command.Execute());
                }
            }
        }

        private void BindTexts(Dictionary<string, IProperty> properties)
        {
            foreach (var text in _view.Texts)
            {
                if (properties.TryGetValue(text.Key, out var property))
                {
                    _textPropertyMap[text.Value] = property;
                    UpdateTextBinding(text.Value, property);
                    property.Changed += _ => UpdateTextBinding(text.Value, property);
                }
            }
        }

        private void BindImages(Dictionary<string, IProperty> properties)
        {
            foreach (var image in _view.Images)
            {
                if (properties.TryGetValue(image.Key, out var property))
                {
                    _imagePropertyMap[image.Value] = property;
                    UpdateImageBinding(image.Value, property);
                    property.Changed += _ => UpdateImageBinding(image.Value, property);
                }
            }
        }

        private void BindSliders(Dictionary<string, IProperty> properties)
        {
            foreach (var slider in _view.Sliders)
            {
                if (properties.TryGetValue(slider.Key, out var property))
                {
                    _sliderPropertyMap[slider.Value] = property;
                    UpdateSliderBinding(slider.Value, property);
                    property.Changed += _ => UpdateSliderBinding(slider.Value, property);
                    slider.Value.onValueChanged.AddListener(value => property.Set(value));
                }
            }
        }

        public void Unbind()
        {
            foreach (var button in _buttonCommandMap.Keys)
                button.onClick.RemoveAllListeners();

            foreach (var textPropertyPair in _textPropertyMap)
                textPropertyPair.Value.Changed -= _ => UpdateTextBinding(textPropertyPair.Key, textPropertyPair.Value);

            foreach (var imagePropertyPair in _imagePropertyMap)
                imagePropertyPair.Value.Changed -= _ => UpdateImageBinding(imagePropertyPair.Key, imagePropertyPair.Value);

            foreach (var sliderPropertyPair in _sliderPropertyMap)
                sliderPropertyPair.Value.Changed -= _ => UpdateSliderBinding(sliderPropertyPair.Key, sliderPropertyPair.Value);

            _buttonCommandMap.Clear();
            _textPropertyMap.Clear();
            _imagePropertyMap.Clear();
            _sliderPropertyMap.Clear();
        }

        private void UpdateTextBinding(TMP_Text text, IProperty property)
        {
            text.text = property.Get()?.ToString() ?? string.Empty;
        }

        private void UpdateImageBinding(Image image, IProperty property)
        {
            image.sprite = property.Get() as Sprite;
        }

        private void UpdateSliderBinding(Slider slider, IProperty property)
        {
            if (property.Get() is float newValue)
                slider.value = newValue;
        }
    }
}