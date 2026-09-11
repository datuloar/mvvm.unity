using MvvmUnity.Unity;

namespace MvvmUnity.Tests.Generation
{
    public partial class IntentView
    {
        public bool CustomBindCalled { get; private set; }

        partial void BindCustom(BindingScope bindings, IntentViewModel viewModel)
        {
            CustomBindCalled = true;
        }
    }
}
