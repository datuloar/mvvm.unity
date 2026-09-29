using System.Collections.Generic;
using System.Linq;

using MvvmUnity.Editor;
using MvvmUnity.Tests.Fixtures;
using MvvmUnity.Unity;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class ViewBindingAnalyzerTests
    {
        [Test]
        public void ConventionsAndInferredObserversProduceAPlan()
        {
            var errors = new List<string>();

            Assert.That(ViewBindingAnalyzer.TryAnalyze(typeof(IntentView), true, errors, out var plan), Is.True);
            Assert.That(errors, Is.Empty);

            var field = plan.Fields.Single();
            Assert.That(field.Field, Is.EqualTo("_submit"));
            Assert.That(field.Source, Is.EqualTo(nameof(IntentViewModel.Submit)));
            Assert.That(field.Target, Is.EqualTo(BindingTarget.Click));
            Assert.That(plan.Observers.Select(observer => observer.Source), Is.EqualTo(new[] { "Status", "Attempts" }));
        }

        [Test]
        public void MatchingNameWithIncompatibleWidgetIsAnError()
        {
            var errors = new List<string>();

            Assert.That(ViewBindingAnalyzer.TryAnalyze(typeof(MismatchedView), true, errors, out _), Is.False);
            Assert.That(errors.Single(), Does.Contain("MismatchedView._submit"));
        }

        [Test]
        public void DisabledConventionsIgnoreMatchingNames()
        {
            var errors = new List<string>();

            Assert.That(ViewBindingAnalyzer.TryAnalyze(typeof(MismatchedView), false, errors, out _), Is.False);
            Assert.That(errors.Single(), Does.Contain("nothing to generate"));
        }

        [Test]
        public void ViewWithoutBindingsIsRejected()
        {
            var errors = new List<string>();

            Assert.That(ViewBindingAnalyzer.TryAnalyze(typeof(EmptyView), true, errors, out _), Is.False);
            Assert.That(errors.Single(), Does.Contain("EmptyView"));
        }
    }
}
