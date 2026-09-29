using System.Collections.Generic;

using MvvmUnity.Core;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class StateViewModelTests
    {
        [Test]
        public void PublishReplacesSnapshot()
        {
            var viewModel = new ListViewModel();
            var received = new List<string>();

            using (viewModel.State.Subscribe(state => received.Add(string.Join(",", state)), false))
                viewModel.Replace(new List<string> { "a" });

            Assert.That(received, Is.EqualTo(new[] { "a" }));
        }

        [Test]
        public void RepublishNotifiesForInPlaceChanges()
        {
            var viewModel = new ListViewModel();
            var received = new List<string>();

            using (viewModel.State.Subscribe(state => received.Add(string.Join(",", state)), false))
                viewModel.Append("b");

            Assert.That(received, Is.EqualTo(new[] { "b" }));
        }

        private sealed class ListViewModel : StateViewModel<List<string>>
        {
            public ListViewModel()
                : base(new List<string>())
            {
            }

            public void Replace(List<string> items) => Publish(items);

            public void Append(string item)
            {
                State.Value.Add(item);
                Republish();
            }
        }
    }
}
