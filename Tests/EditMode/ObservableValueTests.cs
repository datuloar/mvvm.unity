using System;

using MvvmUnity.Core;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class ObservableValueTests
    {
        [Test]
        public void ParameterlessConstructorStartsWithDefault()
        {
            Assert.That(new ObservableValue<int>().Value, Is.Zero);
        }

        [Test]
        public void SetNotifiesOnlyForChangedValues()
        {
            var value = new ObservableValue<int>(1);
            var notifications = 0;
            value.Changed += _ => notifications++;

            Assert.That(value.Set(1), Is.False);
            Assert.That(value.Set(2), Is.True);
            Assert.That(notifications, Is.EqualTo(1));
        }

        [Test]
        public void CustomComparerDecidesEquality()
        {
            var value = new ObservableValue<string>("a", StringComparer.OrdinalIgnoreCase);
            var notifications = 0;
            value.Changed += _ => notifications++;

            value.Value = "A";

            Assert.That(notifications, Is.Zero);
            Assert.That(value.Value, Is.EqualTo("a"));
        }

        [Test]
        public void RefreshRepublishesCurrentValue()
        {
            var value = new ObservableValue<int>(7);
            var received = 0;
            value.Changed += current => received = current;

            value.Refresh();

            Assert.That(received, Is.EqualTo(7));
        }

        [Test]
        public void SubscribeEmitsCurrentValueAndStopsAfterDispose()
        {
            var value = new ObservableValue<int>(1);
            var received = 0;
            var subscription = value.Subscribe(current => received = current);

            Assert.That(received, Is.EqualTo(1));
            subscription.Dispose();
            value.Value = 2;
            Assert.That(received, Is.EqualTo(1));
        }

        [Test]
        public void SubscribeCanSkipCurrentValue()
        {
            var value = new ObservableValue<int>(1);
            var notifications = 0;

            using (value.Subscribe(_ => notifications++, false))
                Assert.That(notifications, Is.Zero);
        }
    }
}
