using MvvmUnity.Tests.Fixtures;
using NUnit.Framework;
using UnityEngine;

namespace MvvmUnity.Tests
{
    public sealed class MvvmViewTests
    {
        private GameObject _root;
        private ProbeView _view;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("ProbeView");
            _view = _root.AddComponent<ProbeView>();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_root);

        [Test]
        public void SetViewModelBindsAnActiveView()
        {
            var viewModel = new ProbeViewModel();

            _view.SetViewModel(viewModel);

            Assert.That(_view.Binds, Is.EqualTo(1));
            Assert.That(_view.Bound, Is.SameAs(viewModel));
        }

        [Test]
        public void ReassigningTheOwnedViewModelKeepsItAlive()
        {
            var viewModel = new ProbeViewModel();

            _view.SetViewModel(viewModel, true);
            _view.SetViewModel(viewModel, true);

            Assert.That(viewModel.Disposals, Is.Zero);
            Assert.That(_view.Binds, Is.EqualTo(2));
        }

        [Test]
        public void ReplacingAnOwnedViewModelDisposesIt()
        {
            var first = new ProbeViewModel();
            var second = new ProbeViewModel();

            _view.SetViewModel(first, true);
            _view.SetViewModel(second);

            Assert.That(first.Disposals, Is.EqualTo(1));
            Assert.That(second.Disposals, Is.Zero);
        }

        [Test]
        public void ExternallyOwnedViewModelIsNotDisposed()
        {
            var first = new ProbeViewModel();

            _view.SetViewModel(first);
            _view.SetViewModel(new ProbeViewModel());

            Assert.That(first.Disposals, Is.Zero);
        }
    }
}
