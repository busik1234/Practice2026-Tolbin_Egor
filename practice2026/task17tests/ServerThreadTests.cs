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
        [Fact]
        public void RoundRobinScheduler_HandlesEmptyAndSingleCommandStates()
        {
            var scheduler = new RoundRobinScheduler();
            var cmd = new ImplementationBigCommand(2);

            Assert.False(scheduler.HasCommand());

            scheduler.Add(cmd);
            Assert.True(scheduler.HasCommand());
            Assert.Same(cmd, scheduler.Select());

            Assert.False(scheduler.HasCommand());
        }

        [Fact]
        public void ServerThread_ExecutesBigCommands_ToCorrectQuantumCount()
        {
            var server = new ServerThread();

            var cmd1 = new ImplementationBigCommand(3);
            var cmd2 = new ImplementationBigCommand(2);

            server.Start();

            server.AddCommand(cmd1);
            server.AddCommand(cmd2);
            server.AddCommand(new SoftStopCommand(server));

            server.Join();

            Assert.Equal(3, cmd1.CountImplementation);
            Assert.Equal(2, cmd2.CountImplementation);
        }

        [Fact]
        public void ServerThread_StopsBigCommandImmediately_OnHardStop()
        {
            var server = new ServerThread();

            int hugeIterations = 10_000_000;
            var longCmd = new ImplementationBigCommand(hugeIterations);

            server.Start();

            server.AddCommand(longCmd);

            Thread.Sleep(50);

            server.AddCommand(new HardStopCommand(server));

            server.Join();
            Assert.True(longCmd.CountImplementation < hugeIterations);
            Assert.True(longCmd.CountImplementation > 0);
        }
    }
}