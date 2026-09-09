using MvvmUnity.Core;

namespace MvvmUnity.Samples
{
    public sealed class CoinCounterViewModel : ViewModel
    {
        private readonly ICoinsCounterModel _model;

        public CoinCounterViewModel(ICoinsCounterModel model)
        {
            _model = model ?? throw new System.ArgumentNullException(nameof(model));
            Count = new ObservableValue<string>(Format(_model.Coins));
            IsEven = new ObservableValue<bool>(_model.Coins % 2 == 0);
            Slider = new ObservableValue<float>(0f);
            Increase = new RelayCommand(() => _model.AddCoins(1));
            _model.Changed += OnCoinsChanged;
        }

        public ObservableValue<string> Count { get; }
        public ObservableValue<bool> IsEven { get; }
        public ObservableValue<float> Slider { get; }
        public RelayCommand Increase { get; }

        protected override void OnDispose()
        {
            _model.Changed -= OnCoinsChanged;
        }

        private void OnCoinsChanged(int value)
        {
            Count.Value = Format(value);
            IsEven.Value = value % 2 == 0;
        }

        private static string Format(int value)
        {
            return "Coins — " + value;
        }
    }
}
