using System.Collections.Generic;

using MvvmUnity.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Samples.CharacterCreator
{
    [GenerateBindings]
    public sealed partial class CharacterCreatorView : MvvmView<CharacterCreatorViewModel>
    {
        [SerializeField] private TMP_InputField _name;
        [SerializeField] private GameObject _nameMissing;
        [IgnoreBinding]
        [SerializeField] private TMP_Dropdown _classIndex;
        [SerializeField] private Image _classColor;
        [SerializeField] private TMP_Text _classInitial;
        [SerializeField] private TMP_Text _classDescription;
        [SerializeField] private TMP_Text _pointsLeft;
        [SerializeField] private Image _pointsSpent;
        [SerializeField] private StatRowView _rowTemplate;
        [SerializeField] private Transform _rows;
        [SerializeField] private Slider _difficulty;
        [SerializeField] private TMP_Text _difficultyLabel;
        [SerializeField] private Toggle _hardcore;
        [SerializeField] private TMP_Text _status;
        [SerializeField] private TMP_Text _partySize;

        [Bind(nameof(CharacterCreatorViewModel.Randomize))]
        [Bind(nameof(CharacterCreatorViewModel.CanRandomize))]
        [SerializeField] private Button _randomize;

        [SerializeField] private Button _reset;
        [SerializeField] private Button _create;

        [Bind(nameof(CharacterCreatorViewModel.ConfirmationVisible))]
        [SerializeField] private CanvasGroup _confirmationPanel;

        [SerializeField] private TMP_Text _confirmation;
        [SerializeField] private Button _dismiss;

        private readonly List<StatRowView> _rowViews = new List<StatRowView>();

        protected override void Bind(BindingScope bindings, CharacterCreatorViewModel viewModel) =>
            bindings.Choice(_classIndex, viewModel.ClassNames, viewModel.ClassIndex);

        [Observe]
        private void Render(StatSheetState sheet, BindingScope bindings)
        {
            for (var index = 0; index < sheet.Rows.Count; index++)
                RowAt(index).Render(sheet.Rows[index], ViewModel.Increase, ViewModel.Decrease, bindings);
        }

        private StatRowView RowAt(int index)
        {
            while (_rowViews.Count <= index)
            {
                var row = Instantiate(_rowTemplate, _rows);
                row.gameObject.SetActive(true);
                _rowViews.Add(row);
            }

            return _rowViews[index];
        }
    }
}
