using System;
using System.Threading;
using System.Windows.Input;
using task17;
using Xunit;

namespace task17.tests
{
    public class ServerThreadTests
    {
        public class IncrementCommand : ICommand
        {
            public int ExecutionCount { get; set; } = 0;
            public void Execute() => ExecutionCount++;
        }

        [Fact]
        public void HardStop_ShouldStopImmediately_IgnoringRemainingCommands()
        {
            var server = new ServerThread();
            var cmd1 = new IncrementCommand();
            var cmd2 = new IncrementCommand();
            var hardStop = new HardStopCommand(server);

            server.Start();

            server.AddCommand(cmd1);
            server.AddCommand(hardStop);
            server.AddCommand(cmd2);

            server.Join();

            Assert.Equal(1, cmd1.ExecutionCount);
            Assert.Equal(0, cmd2.ExecutionCount);
        }

        [Fact]
        public void SoftStop_ShouldExecuteAllExistingCommands_ThenStop()
        {
            var server = new ServerThread();
            var cmd1 = new IncrementCommand();
            var cmd2 = new IncrementCommand();
            var softStop = new SoftStopCommand(server);

            server.Start();

            server.AddCommand(cmd1);
            server.AddCommand(softStop);

            Thread.Sleep(50);

            server.AddCommand(cmd2);

            server.Join();

            Assert.Equal(1, cmd1.ExecutionCount);
            Assert.Equal(0, cmd2.ExecutionCount);
        }

        [Fact]
        public void StopCommands_ShouldThrowException_WhenExecutedFromMainThread()
        {
            var server = new ServerThread();
            var hardStop = new HardStopCommand(server);
            var softStop = new SoftStopCommand(server);

            Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
            Assert.Throws<InvalidOperationException>(() => softStop.Execute());
        }
    }
}