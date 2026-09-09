using MvvmUnity.Core;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class ObservableValueTests
    {
        [Test]
        public void Value_NotifiesOnlyForChanges()
        {
            var value = new ObservableValue<int>(1);
            var notifications = 0;
            var subscription = value.Subscribe(_ => notifications++);

            value.Value = 1;
            value.Value = 2;

            Assert.AreEqual(2, notifications);
            subscription.Dispose();
            value.Value = 3;
            Assert.AreEqual(2, notifications);
        }
    }
}
