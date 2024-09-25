using mvvm.unity.Core;
using System.Collections.Generic;
using UnityEngine;

namespace mvvm.unity.Samples
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private Window _coinsWindow;

        private WindowsManager<SampleWindowType> _windowsManager;

        private async void Start()
        {
            var windows = new Dictionary<SampleWindowType, IWindow>()
            {
                { SampleWindowType.Coin, _coinsWindow },
            };

            var windowsProvider = new WindowsProvider<SampleWindowType>(windows);
            _windowsManager = new WindowsManager<SampleWindowType>(windowsProvider);

            var coinCounterViewModel = new CoinCounterViewModel(new CoinsCounterModel());
            await _windowsManager.BindAndShowAsync(SampleWindowType.Coin, coinCounterViewModel);
        }

        private async void OnDestroy()
        {
            await _windowsManager.HideAndUnbindAsync(SampleWindowType.Coin);
        }
    }
}