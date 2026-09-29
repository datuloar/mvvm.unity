using MvvmUnity.Core;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class ViewModelTests
    {
        [Test]
        public void DisposeReleasesOwnedResourcesOnce()
        {
            var source = new ObservableValue<int>();
            var viewModel = new CounterViewModel(source);

            viewModel.Dispose();
            viewModel.Dispose();
            source.Value = 5;

            Assert.That(viewModel.Received, Is.Zero);
            Assert.That(viewModel.Disposals, Is.EqualTo(1));
        }

        [Test]
        public void OwnReturnsTheResource()
        {
            var source = new ObservableValue<int>();
            var viewModel = new CounterViewModel(source);

            source.Value = 3;

            Assert.That(viewModel.Received, Is.EqualTo(3));
            Assert.That(viewModel.Command, Is.Not.Null);
        }

        private sealed class CounterViewModel : ViewModel
        {
            public CounterViewModel(IReadOnlyObservableValue<int> source)
            {
                Own(source.Subscribe(value => Received = value));
                Command = Own(new RelayCommand(() => { }).RefreshOn(source));
            }

            public RelayCommand Command { get; }

            public int Received { get; private set; }

            public int Disposals { get; private set; }

            protected override void OnDispose() => Disposals++;
        }
    }
}
