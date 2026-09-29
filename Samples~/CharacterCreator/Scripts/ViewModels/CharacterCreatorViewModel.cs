using System;
using System.Collections.Generic;

using MvvmUnity.Core;
using UnityEngine;

using Random = System.Random;

namespace MvvmUnity.Samples.CharacterCreator
{
    public sealed class CharacterCreatorViewModel : StateViewModel<StatSheetState>
    {
        private static readonly string[] RandomNames = { "Aria", "Borin", "Cael", "Dara", "Eldric", "Fenna", "Garr", "Ilse" };

        private readonly HeroDraft _draft;
        private readonly IHeroRoster _roster;
        private readonly Random _random;
        private readonly ObservableValue<string> _name = new ObservableValue<string>(string.Empty);
        private readonly ObservableValue<int> _classIndex = new ObservableValue<int>();
        private readonly ObservableValue<float> _difficulty = new ObservableValue<float>(0.5f);
        private readonly ObservableValue<bool> _hardcore = new ObservableValue<bool>();
        private readonly ObservableValue<Color> _classColor = new ObservableValue<Color>();
        private readonly ObservableValue<string> _classInitial = new ObservableValue<string>();
        private readonly ObservableValue<string> _classDescription = new ObservableValue<string>();
        private readonly ObservableValue<string> _difficultyLabel = new ObservableValue<string>();
        private readonly ObservableValue<bool> _canRandomize = new ObservableValue<bool>();
        private readonly ObservableValue<string> _pointsLeft = new ObservableValue<string>();
        private readonly ObservableValue<float> _pointsSpent = new ObservableValue<float>();
        private readonly ObservableValue<bool> _nameMissing = new ObservableValue<bool>();
        private readonly ObservableValue<string> _status = new ObservableValue<string>();
        private readonly ObservableValue<string> _partySize = new ObservableValue<string>();
        private readonly ObservableValue<bool> _confirmationVisible = new ObservableValue<bool>();
        private readonly ObservableValue<string> _confirmation = new ObservableValue<string>(string.Empty);
        private readonly RelayCommand _create;
        private readonly RelayCommand<HeroStat> _increase;
        private readonly RelayCommand<HeroStat> _decrease;

        public CharacterCreatorViewModel(HeroDraft draft, IHeroRoster roster, Random random)
            : base(StatSheetState.From(draft))
        {
            _draft = draft ?? throw new ArgumentNullException(nameof(draft));
            _roster = roster ?? throw new ArgumentNullException(nameof(roster));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _create = Own(new RelayCommand(CreateHero, CanCreate).RefreshOn(_name));
            _increase = Own(new RelayCommand<HeroStat>(_draft.Increase, _draft.CanIncrease));
            _decrease = Own(new RelayCommand<HeroStat>(_draft.Decrease, _draft.CanDecrease));

            Own(_classIndex.Subscribe(OnClassChanged));
            Own(_difficulty.Subscribe(OnDifficultyChanged));
            Own(_hardcore.Subscribe(hardcore => _canRandomize.Value = !hardcore));
            Own(_name.Subscribe(_ => RefreshValidation()));
            _draft.Changed += OnDraftChanged;
            OnDraftChanged();
            RefreshParty();
        }

        public IObservableValue<string> Name => _name;

        public IObservableValue<int> ClassIndex => _classIndex;

        public IReadOnlyList<string> ClassNames => HeroClassCatalog.Names;

        public IObservableValue<float> Difficulty => _difficulty;

        public IObservableValue<bool> Hardcore => _hardcore;

        public IReadOnlyObservableValue<Color> ClassColor => _classColor;

        public IReadOnlyObservableValue<string> ClassInitial => _classInitial;

        public IReadOnlyObservableValue<string> ClassDescription => _classDescription;

        public IReadOnlyObservableValue<string> DifficultyLabel => _difficultyLabel;

        public IReadOnlyObservableValue<bool> CanRandomize => _canRandomize;

        public IReadOnlyObservableValue<string> PointsLeft => _pointsLeft;

        public IReadOnlyObservableValue<float> PointsSpent => _pointsSpent;

        public IReadOnlyObservableValue<bool> NameMissing => _nameMissing;

        public IReadOnlyObservableValue<string> Status => _status;

        public IReadOnlyObservableValue<string> PartySize => _partySize;

        public IReadOnlyObservableValue<bool> ConfirmationVisible => _confirmationVisible;

        public IReadOnlyObservableValue<string> Confirmation => _confirmation;

        public ICommand Create => _create;

        public ICommand<HeroStat> Increase => _increase;

        public ICommand<HeroStat> Decrease => _decrease;

        private HeroClass SelectedClass => (HeroClass)Mathf.Clamp(_classIndex.Value, 0, HeroClassCatalog.Names.Count - 1);

        public void Randomize()
        {
            if (!_canRandomize.Value)
                return;

            _name.Value = RandomNames[_random.Next(RandomNames.Length)];
            _classIndex.Value = _random.Next(HeroClassCatalog.Names.Count);
            _draft.Distribute(_random);
        }

        public void Reset()
        {
            _name.Value = string.Empty;
            _classIndex.Value = 0;
            _difficulty.Value = 0.5f;
            _hardcore.Value = false;
            _draft.Clear();
        }

        public void Dismiss() => _confirmationVisible.Value = false;

        protected override void OnDispose() => _draft.Changed -= OnDraftChanged;

        private bool CanCreate() => !IsNameMissing() && _draft.RemainingPoints == 0;

        private bool IsNameMissing() => string.IsNullOrWhiteSpace(_name.Value);

        private void CreateHero()
        {
            var hero = new HeroProfile(
                _name.Value.Trim(),
                SelectedClass,
                _draft.Points(HeroStat.Strength),
                _draft.Points(HeroStat.Agility),
                _draft.Points(HeroStat.Intellect),
                _hardcore.Value);
            _roster.Add(hero);
            _confirmation.Value = $"{hero.Name} the {HeroClassCatalog.Name(hero.HeroClass)} joined the party";
            _confirmationVisible.Value = true;
            RefreshParty();
        }

        private void OnDraftChanged()
        {
            Publish(StatSheetState.From(_draft));
            _pointsLeft.Value = $"{_draft.RemainingPoints} / {HeroDraft.PointBudget} points left";
            _pointsSpent.Value = (float)_draft.SpentPoints / HeroDraft.PointBudget;
            _increase.Refresh();
            _decrease.Refresh();
            RefreshValidation();
        }

        private void OnClassChanged(int _)
        {
            var heroClass = SelectedClass;
            _classColor.Value = HeroClassCatalog.Tint(heroClass);
            _classInitial.Value = HeroClassCatalog.Name(heroClass).Substring(0, 1);
            _classDescription.Value = HeroClassCatalog.Description(heroClass);
        }

        private void OnDifficultyChanged(float difficulty)
        {
            var tier = difficulty < 0.34f ? "Story" : difficulty < 0.67f ? "Veteran" : "Nightmare";
            _difficultyLabel.Value = $"{tier} · {Mathf.RoundToInt(difficulty * 100f)}%";
        }

        private void RefreshValidation()
        {
            _nameMissing.Value = IsNameMissing();
            _status.Value = IsNameMissing()
                ? "Name your hero to continue"
                : _draft.RemainingPoints > 0
                    ? $"Spend {_draft.RemainingPoints} more points"
                    : "Ready to join the party";
            _create.Refresh();
        }

        private void RefreshParty() =>
            _partySize.Value = _roster.Count == 1 ? "Party: 1 hero" : $"Party: {_roster.Count} heroes";
    }
}
