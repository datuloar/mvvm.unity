using MvvmUnity.Unity;

namespace MvvmUnity.Tests.Fixtures
{
    public partial class IntentView
    {
        [global::System.CodeDom.Compiler.GeneratedCode("MvvmUnity.BindingCodeGenerator", "2.0")]
        protected override void BindGenerated(
            global::MvvmUnity.Unity.BindingScope bindings,
            global::MvvmUnity.Tests.Fixtures.IntentViewModel viewModel)
        {
            bindings.Click(_submit, viewModel.Submit);
            bindings.Observe(viewModel.Status, Render);
            bindings.Observe(viewModel.Attempts, RenderAttempts);
        }
    }
}
