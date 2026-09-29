using MvvmUnity.Core;

namespace MvvmUnity.Tests.Fixtures
{
    public sealed class IntentViewModel : ViewModel
    {
        private readonly ObservableValue<string> _status = new ObservableValue<string>(string.Empty);
        private readonly ObservableValue<int> _attempts = new ObservableValue<int>();

        public IReadOnlyObservableValue<string> Status => _status;

        public IReadOnlyObservableValue<int> Attempts => _attempts;

        public void Submit()
        {
            _attempts.Value++;
            _status.Value = "Submitted " + _attempts.Value;
        }
    }
}
