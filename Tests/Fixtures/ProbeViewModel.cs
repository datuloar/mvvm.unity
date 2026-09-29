using MvvmUnity.Core;

namespace MvvmUnity.Tests.Fixtures
{
    public sealed class ProbeViewModel : ViewModel
    {
        public int Disposals { get; private set; }

        protected override void OnDispose() => Disposals++;
    }
}
