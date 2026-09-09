using MvvmUnity.Core;

namespace MvvmUnity.Tests.Generation
{
    public sealed class IntentViewModel : ViewModel
    {
        private readonly ObservableValue<string> _status = new ObservableValue<string>(string.Empty);

        public int Executions { get; private set; }

        public IReadOnlyObservableValue<string> Status => _status;

        public void Submit()
        {
            Executions++;
            _status.Value = Executions.ToString();
        }
    }
}
