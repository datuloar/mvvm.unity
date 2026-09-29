using MvvmUnity.Unity;

namespace MvvmUnity.Tests.Fixtures
{
    public sealed class ProbeView : MvvmView<ProbeViewModel>
    {
        public int Binds { get; private set; }

        public ProbeViewModel Bound { get; private set; }

        protected override void Bind(BindingScope bindings, ProbeViewModel viewModel)
        {
            Binds++;
            Bound = viewModel;
        }
    }
}
