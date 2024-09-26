using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;

namespace mvvm.unity.Core
{
    public interface IView
    {
        IReadOnlyDictionary<string, Button> Buttons { get; }
        IReadOnlyDictionary<string, TMP_Text> Texts { get; }
        IReadOnlyDictionary<string, Image> Images { get; }
        IReadOnlyDictionary<string, Slider> Sliders { get; }
        IReadOnlyDictionary<string, GameObject> GameObjects { get; }

        void Initialize();

        Task HideAsync();
        Task ShowAsync();
    }
}