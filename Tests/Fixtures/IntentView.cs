using MvvmUnity.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Tests.Fixtures
{
    [GenerateBindings]
    public sealed partial class IntentView : MvvmView<IntentViewModel>
    {
        [SerializeField] private Button _submit;

        [IgnoreBinding]
        [SerializeField] private Text _status;

        public string RenderedStatus { get; private set; } = string.Empty;

        public int RenderedAttempts { get; private set; }

        public BindingScope AttemptsScope { get; private set; }

        public int CustomBindings { get; private set; }

        protected override void Bind(BindingScope bindings, IntentViewModel viewModel) => CustomBindings++;

        [Observe]
        private void Render(string status) => RenderedStatus = status;

        [Observe]
        private void RenderAttempts(int attempts, BindingScope bindings)
        {
            RenderedAttempts = attempts;
            AttemptsScope = bindings;
        }
    }
}
