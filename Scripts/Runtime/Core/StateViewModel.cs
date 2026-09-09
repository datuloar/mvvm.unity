namespace MvvmUnity.Core
{
    public abstract class StateViewModel<TState> : ViewModel
    {
        private readonly ObservableValue<TState> _state;

        protected StateViewModel(TState initialState)
        {
            _state = new ObservableValue<TState>(initialState);
        }

        public IReadOnlyObservableValue<TState> State => _state;

        protected void Publish(TState state)
        {
            _state.Value = state;
        }

        /// Для случая, когда состояние меняют на месте, а не заменяют новым экземпляром.
        protected void Republish()
        {
            _state.Refresh();
        }
    }
}
