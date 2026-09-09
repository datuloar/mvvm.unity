using MvvmUnity.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Samples
{
    [GenerateBindings]
    public sealed partial class CoinCounterView : MvvmView<CoinCounterViewModel>
    {
        [SerializeField] private TMP_Text _count;

        [Bind(nameof(CoinCounterViewModel.IsEven))]
        [SerializeField] private GameObject _evenBadge;

        [SerializeField] private Slider _slider;

        [SerializeField] private Button _increase;
    }
}
