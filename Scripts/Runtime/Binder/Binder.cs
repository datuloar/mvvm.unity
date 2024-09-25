using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace mvvm.unity.Core
{
    public class Binder
    {
        private readonly View _view;
        private readonly ViewModel _viewModel;

        private readonly Dictionary<Button, ICommand> _buttonCommandMap = new();
        private readonly Dictionary<TMP_Text, IProperty> _textPropertyMap = new();
        private readonly Dictionary<Image, IProperty> _imagePropertyMap = new();
        private readonly Dictionary<Slider, IProperty> _sliderPropertyMap = new();

        public Binder(View view, ViewModel viewModel)
        {
            _view = view;
            _viewModel = viewModel;
        }

        public void Bind()
        {
            foreach (var button in _view.Buttons)
            {
                var command = _viewModel.GetCommands().FirstOrDefault(cmd => cmd.Name == button.Key);

                if (command != null)
                {
                    _buttonCommandMap[button.Value] = command;
                    button.Value.onClick.AddListener(() => command.Execute());
                }
            }

            foreach (var text in _view.Texts)
            {
                var property = _viewModel.GetProperties().FirstOrDefault(prop => prop.Name == text.Key);

                if (property != null)
                {
                    _textPropertyMap[text.Value] = property;
                    UpdateTextBinding(text.Value, property);

                    property.Changed += value => UpdateTextBinding(text.Value, property);
                }
            }

            foreach (var image in _view.Images)
            {
                var property = _viewModel.GetProperties().FirstOrDefault(prop => prop.Name == image.Key);

                if (property != null)
                {
                    _imagePropertyMap[image.Value] = property;
                    UpdateImageBinding(image.Value, property);

                    property.Changed += value => UpdateImageBinding(image.Value, property);
                }
            }

            foreach (var slider in _view.Sliders)
            {
                var property = _viewModel.GetProperties().FirstOrDefault(prop => prop.Name == slider.Key);

                if (property != null)
                {
                    _sliderPropertyMap[slider.Value] = property;
                    UpdateSliderBinding(slider.Value, property);

                    property.Changed += value => UpdateSliderBinding(slider.Value, property);
                    slider.Value.onValueChanged.AddListener(value => property.Set(value));
                }
            }
        }

        public void Unbind()
        {
            foreach (var button in _buttonCommandMap.Keys)
                button.onClick.RemoveAllListeners();

            foreach (var textPropertyPair in _textPropertyMap)
                textPropertyPair.Value.Changed -= value =>
                    UpdateTextBinding(textPropertyPair.Key, textPropertyPair.Value);

            foreach (var imagePropertyPair in _imagePropertyMap)
                imagePropertyPair.Value.Changed -= value =>
                    UpdateImageBinding(imagePropertyPair.Key, imagePropertyPair.Value);

            foreach (var sliderPropertyPair in _sliderPropertyMap)
                sliderPropertyPair.Value.Changed -= value =>
                    UpdateSliderBinding(sliderPropertyPair.Key, sliderPropertyPair.Value);

            _buttonCommandMap.Clear();
            _textPropertyMap.Clear();
            _imagePropertyMap.Clear();
            _sliderPropertyMap.Clear();
        }

        private void UpdateTextBinding(TMP_Text text, IProperty property)
        {
            if (property.Get() is string newText)
                text.text = newText;
            else
                text.text = property.Get()?.ToString() ?? string.Empty;
        }

        private void UpdateImageBinding(Image image, IProperty property)
        {
            if (property.Get() is Sprite newSprite)
                image.sprite = newSprite;
        }

        private void UpdateSliderBinding(Slider slider, IProperty property)
        {
            if (property.Get() is float newValue)
                slider.value = newValue;
        }
    }
}