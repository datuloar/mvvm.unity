using MvvmUnity.Core;
using MvvmUnity.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Samples.CharacterCreator
{
    public sealed class StatRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TMP_Text _points;
        [SerializeField] private Image _meter;
        [SerializeField] private Button _decrease;
        [SerializeField] private Button _increase;

        public void Render(
            StatRowState row,
            ICommand<HeroStat> increase,
            ICommand<HeroStat> decrease,
            BindingScope bindings)
        {
            _label.text = row.Label;
            _points.text = row.Points;
            _meter.fillAmount = row.Meter;
            bindings.Command(_increase, increase, row.Stat);
            bindings.Command(_decrease, decrease, row.Stat);
        }
    }
}
