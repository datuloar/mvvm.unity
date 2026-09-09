using MvvmUnity.Core;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class StateViewModelTests
    {
        [Test]
        public void PublishReplacesSnapshotAndNotifiesObserver()
        {
            var viewModel = new TestViewModel();
            var observed = "";
            var subscription = viewModel.State.Subscribe(value => observed = value);

            viewModel.Change("ready");

            Assert.AreEqual("ready", observed);
            subscription.Dispose();
        }

        private sealed class TestViewModel : StateViewModel<string>
        {
            public TestViewModel()
                : base("")
            {
            }

            public void Change(string value)
            {
                Publish(value);
            }
        }
    }
}
