using mvvm.unity.Core;

namespace mvvm.unity.Samples
{
    public class CoinCounterViewModel : ViewModel
    {
        private ICoinsCounterModel _model;
        private float _slider;

        public CoinCounterViewModel(ICoinsCounterModel model)
        {
            _model = model;
            _model.Changed += OnCoinsChanged;
        }

        ~CoinCounterViewModel()
        {
            _model.Changed -= OnCoinsChanged;
        }

        [BindableProperty] public bool IsCoinsDivideTwo => _model.Coins % 2 == 0;
        [BindableProperty] public string CoinCount => "Coins - " + ValueFormatter.Format(_model.Coins);
        [BindableProperty] public string SliderValue => "Slider - " + Slider;    
        [BindableProperty] public float Slider
        {
            get => _slider;
            set
            {
                if (_slider != value)
                {
                    _slider = value;
                    OnPropertyChanged(nameof(SliderValue));
                }
            }
        }

        [BindableCommand] public void IncreaseCoinCount() => _model.AddCoins(1);

        private void OnCoinsChanged(int _)
        {
            OnPropertyChanged(nameof(CoinCount));
            OnPropertyChanged(nameof(IsCoinsDivideTwo));
        }
    }
}