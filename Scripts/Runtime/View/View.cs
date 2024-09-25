using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading.Tasks;

namespace mvvm.unity.Core
{
    public class View : MonoBehaviour
    {
        private const string BindableButtonPrefix = "[BindableBtn]";
        private const string BindableTextPrefix = "[BindableTxt]";
        private const string BindableImagePrefix = "[BindImg]";
        private const string BindableSliderPrefix = "[BindableSld]";

        [SerializeField] private List<Image> _images;
        [SerializeField] private List<TMP_Text> _texts;
        [SerializeField] private List<Button> _buttons;
        [SerializeField] private List<Slider> _sliders;

        private Dictionary<string, Button> _buttonsMap = new();
        private Dictionary<string, TMP_Text> _textsMap = new();
        private Dictionary<string, Image> _imagesMap = new();
        private Dictionary<string, Slider> _slidersMap = new();

        public IReadOnlyDictionary<string, Button> Buttons => _buttonsMap;
        public IReadOnlyDictionary<string, TMP_Text> Texts => _textsMap;
        public IReadOnlyDictionary<string, Image> Images => _imagesMap;
        public IReadOnlyDictionary<string, Slider> Sliders => _slidersMap;

        private void OnValidate()
        {
            _buttons = GetComponentsInChildren<Button>()
                .Where(b => b.name.StartsWith(BindableButtonPrefix))
                .ToList();

            _texts = GetComponentsInChildren<TMP_Text>()
                .Where(t => t.name.StartsWith(BindableTextPrefix))
                .ToList();

            _images = GetComponentsInChildren<Image>()
                .Where(i => i.name.StartsWith(BindableImagePrefix))
                .ToList();

            _sliders = GetComponentsInChildren<Slider>()
                .Where(s => s.name.StartsWith(BindableSliderPrefix))
                .ToList();
        }

        public void Initialize()
        {
            _buttonsMap = _buttons.ToDictionary(b => b.name.Replace(BindableButtonPrefix, "").Trim(), b => b);
            _textsMap = _texts.ToDictionary(t => t.name.Replace(BindableTextPrefix, "").Trim(), t => t);
            _imagesMap = _images.ToDictionary(i => i.name.Replace(BindableImagePrefix, "").Trim(), i => i);
            _slidersMap = _sliders.ToDictionary(s => s.name.Replace(BindableSliderPrefix, "").Trim(), s => s);
        }

        public async Task ShowAsync()
        {
            gameObject.SetActive(true);
            await OnShowAsync();
        }

        public async Task HideAsync()
        {
            await OnHideAsync();
            gameObject.SetActive(false);
        }

        protected virtual async Task OnShowAsync() { }

        protected virtual async Task OnHideAsync() { }
    }
}