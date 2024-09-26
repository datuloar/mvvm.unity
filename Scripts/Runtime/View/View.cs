using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

namespace mvvm.unity.Core
{
    public class View : MonoBehaviour, IView
    {
        private const string BindableButtonPrefix = "[BindableBtn]";
        private const string BindableTextPrefix = "[BindableTxt]";
        private const string BindableImagePrefix = "[BindImg]";
        private const string BindableSliderPrefix = "[BindableSld]";
        private const string BindableGoPrefix = "[BindableGo]";

        [SerializeField] private List<Image> _images;
        [SerializeField] private List<TMP_Text> _texts;
        [SerializeField] private List<Button> _buttons;
        [SerializeField] private List<Slider> _sliders;
        [SerializeField] private List<GameObject> _gameObjects;

        private Dictionary<string, Button> _buttonsMap = new();
        private Dictionary<string, TMP_Text> _textsMap = new();
        private Dictionary<string, Image> _imagesMap = new();
        private Dictionary<string, Slider> _slidersMap = new();
        private Dictionary<string, GameObject> _gameObjectsMap = new();

        private bool _isInitialized;

        public IReadOnlyDictionary<string, Button> Buttons => _buttonsMap;
        public IReadOnlyDictionary<string, TMP_Text> Texts => _textsMap;
        public IReadOnlyDictionary<string, Image> Images => _imagesMap;
        public IReadOnlyDictionary<string, Slider> Sliders => _slidersMap;
        public IReadOnlyDictionary<string, GameObject> GameObjects => _gameObjectsMap;

        private void OnValidate()
        {
            _buttons.Clear();
            _texts.Clear();
            _images.Clear();
            _sliders.Clear();
            _gameObjects.Clear();

            foreach (var component in GetComponentsInChildren<Transform>(true))
            {
                var go = component.gameObject;

                if (go.name.StartsWith(BindableButtonPrefix) && go.TryGetComponent(out Button button))
                    _buttons.Add(button);
                else if (go.name.StartsWith(BindableTextPrefix) && go.TryGetComponent(out TMP_Text text))
                    _texts.Add(text);
                else if (go.name.StartsWith(BindableImagePrefix) && go.TryGetComponent(out Image image))
                    _images.Add(image);
                else if (go.name.StartsWith(BindableSliderPrefix) && go.TryGetComponent(out Slider slider))
                    _sliders.Add(slider);
                else if (go.name.StartsWith(BindableGoPrefix))
                    _gameObjects.Add(go);
            }

            OnValidateOverride();
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            InitializeDictionary(_buttons, _buttonsMap, BindableButtonPrefix);
            InitializeDictionary(_texts, _textsMap, BindableTextPrefix);
            InitializeDictionary(_images, _imagesMap, BindableImagePrefix);
            InitializeDictionary(_sliders, _slidersMap, BindableSliderPrefix);
            InitializeDictionary(_gameObjects, _gameObjectsMap, BindableGoPrefix);

            _isInitialized = true;
        }

        private void InitializeDictionary<T>(List<T> list, Dictionary<string, T> map, string prefix) where T : UnityEngine.Object
        {
            map.Clear();

            foreach (var item in list)
            {
                var key = item.name.Replace(prefix, "").Trim();
                map[key] = item;
            }
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

        protected virtual void OnValidateOverride() { }
    }
}