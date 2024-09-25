using mvvm.unity.Core;

namespace mvvm.unity.Samples
{
    public class CoinCounterViewModel : ViewModel
    {
        private ICoinsCounterModel _model;

        public CoinCounterViewModel(ICoinsCounterModel model)
        {
            _model = model;
            _model.Changed += OnCoinsChanged;
        }

        ~CoinCounterViewModel()
        {
            _model.Changed -= OnCoinsChanged;
        }

        [BindableProperty] public string CoinCount => ValueFormatter.Format(_model.Coins);

        [BindableCommand] public void IncreaseCoinCount() => _model.AddCoins(1);

        private void OnCoinsChanged(int value) => OnPropertyChanged(nameof(CoinCount));
    }
}