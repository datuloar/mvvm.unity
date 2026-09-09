using UnityEngine;

namespace MvvmUnity.Samples
{
    public sealed class EntryPoint : MonoBehaviour
    {
        [SerializeField] private CoinCounterView _coinsView;

        private void Start()
        {
            _coinsView.gameObject.SetActive(true);
            _coinsView.SetViewModel(new CoinCounterViewModel(new CoinsCounterModel()), true);
        }
    }
}
