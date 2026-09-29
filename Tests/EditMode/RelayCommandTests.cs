using MvvmUnity.Core;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class RelayCommandTests
    {
        [Test]
        public void ExecuteRespectsCanExecute()
        {
            var enabled = false;
            var executions = 0;
            var command = new RelayCommand(() => executions++, () => enabled);

            command.Execute();
            enabled = true;
            command.Execute();

            Assert.That(executions, Is.EqualTo(1));
        }

        [Test]
        public void RefreshRaisesCanExecuteChanged()
        {
            var notifications = 0;
            var command = new RelayCommand(() => { });
            command.CanExecuteChanged += () => notifications++;

            command.Refresh();

            Assert.That(notifications, Is.EqualTo(1));
        }

        [Test]
        public void RefreshOnFollowsTriggerUntilDisposed()
        {
            var name = new ObservableValue<string>(string.Empty);
            var notifications = 0;
            var command = new RelayCommand(() => { }, () => name.Value.Length > 0).RefreshOn(name);
            command.CanExecuteChanged += () => notifications++;

            name.Value = "Ivan";
            Assert.That(command.CanExecute, Is.True);
            Assert.That(notifications, Is.EqualTo(1));

            command.Dispose();
            name.Value = string.Empty;
            Assert.That(notifications, Is.EqualTo(1));
        }

        [Test]
        public void ParameterizedCommandPassesArgument()
        {
            var received = string.Empty;
            ICommand<string> command = new RelayCommand<string>(value => received = value, value => value.Length > 2);

            command.Execute("no");
            command.Execute("ready");

            Assert.That(received, Is.EqualTo("ready"));
            Assert.That(command.CanExecute("no"), Is.False);
        }

        [Test]
        public void ParameterizedRefreshOnFollowsTriggerUntilDisposed()
        {
            var enabled = new ObservableValue<bool>(false);
            var notifications = 0;
            var command = new RelayCommand<string>(_ => { }, _ => enabled.Value).RefreshOn(enabled);
            command.CanExecuteChanged += () => notifications++;

            enabled.Value = true;
            Assert.That(command.CanExecute("station-01"), Is.True);
            Assert.That(notifications, Is.EqualTo(1));

            command.Dispose();
            enabled.Value = false;
            Assert.That(notifications, Is.EqualTo(1));
        }
    }
}
