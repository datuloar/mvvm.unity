using MvvmUnity.Core;
using NUnit.Framework;

namespace MvvmUnity.Tests
{
    public sealed class RelayCommandTests
    {
        [Test]
        public void Execute_RespectsCanExecute()
        {
            var enabled = false;
            var executions = 0;
            var command = new RelayCommand(() => executions++, () => enabled);

            command.Execute();
            enabled = true;
            command.Execute();

            Assert.AreEqual(1, executions);
        }

        [Test]
        public void Refresh_RaisesCanExecuteNotification()
        {
            var notifications = 0;
            var command = new RelayCommand(() => { });
            command.CanExecuteChanged += () => notifications++;

            command.Refresh();

            Assert.AreEqual(1, notifications);
        }

        [Test]
        public void ParameterizedCommand_UsesArgumentAndRaisesNotification()
        {
            var received = string.Empty;
            var notifications = 0;
            ICommand<string> command = new RelayCommand<string>(value => received = value, value => value.Length > 2);
            command.CanExecuteChanged += () => notifications++;

            command.Execute("no");
            command.Execute("ready");
            ((RelayCommand<string>)command).Refresh();

            Assert.AreEqual("ready", received);
            Assert.AreEqual(1, notifications);
        }
    }
}
