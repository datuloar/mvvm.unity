using System.Collections.Generic;

using MvvmUnity.Core;
using MvvmUnity.Unity;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class BindingScopeTests
    {
        [Test]
        public void ObservePushesInitialValueAndStopsAfterDispose()
        {
            var source = new ObservableValue<int>(7);
            var observed = 0;
            var scope = new BindingScope();

            scope.Observe(source, value => observed = value);
            source.Value = 9;
            scope.Dispose();
            source.Value = 11;

            Assert.That(observed, Is.EqualTo(9));
        }

        [Test]
        public void ScopedObserveDisposesPreviousRenderScope()
        {
            var source = new ObservableValue<int>(1);
            var scopes = new List<BindingScope>();
            var released = new List<int>();
            var scope = new BindingScope();

            scope.Observe(source, (value, rows) =>
            {
                scopes.Add(rows);
                rows.Add(new ActionDisposable(() => released.Add(value)));
            });
            source.Value = 2;

            Assert.That(scopes, Has.Count.EqualTo(2));
            Assert.That(released, Is.EqualTo(new[] { 1 }));

            scope.Dispose();
            Assert.That(released, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public void ScopedObserveStopsRenderingAfterDispose()
        {
            var source = new ObservableValue<int>(1);
            var renders = 0;
            var scope = new BindingScope();

            scope.Observe(source, (value, rows) => renders++);
            scope.Dispose();
            source.Value = 2;

            Assert.That(renders, Is.EqualTo(1));
        }

        [Test]
        public void AddAfterDisposeReleasesImmediately()
        {
            var scope = new BindingScope();
            var released = false;
            scope.Dispose();

            scope.Add(new ActionDisposable(() => released = true));

            Assert.That(released, Is.True);
        }
    }
}
