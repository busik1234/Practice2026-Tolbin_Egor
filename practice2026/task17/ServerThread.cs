using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace task17
{
    public interface ICommand
    {
        void Execute();
    }

    public interface IScheduler
    {
        bool HasCommand();
        ICommand Select();
        void Add(ICommand cmd);
    }

    public class ServerThread
    {
        public RoundRobinScheduler Scheduler = new RoundRobinScheduler();
        public BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();
        public Thread thread;
        public bool runcontinue = true;

        public ServerThread()
        {
            thread = new Thread(Runcommand)
            {
                IsBackground = true
            };
        }

        public void Start()
        {
            thread.Start();
        }

        public void AddCommand(ICommand command)
        {
            try
            {
                queue.Add(command);
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void Runcommand()
        {
            while (runcontinue)
            {
                try
                {
                    while (queue.TryTake(out var newCmd))
                    {
                        if (newCmd is HardStopCommand)
                        {
                            newCmd.Execute();
                            runcontinue = false;
                            break;
                        }
                        Scheduler.Add(newCmd);
                    }

                    if (!runcontinue) break;
                    if (Scheduler.HasCommand())
                    {
                        ICommand lastcommand = Scheduler.Select();
                        lastcommand.Execute();

                        if (lastcommand is ImplementationBigCommand bigCmd)
                        {
                            if (bigCmd.CountImplementation < bigCmd.MaxcountImplementation)
                            {
                                Scheduler.Add(bigCmd);
                            }
                        }
                    }
                    else
                    {
                        ICommand incomingCmd = queue.Take();
                        if (incomingCmd is HardStopCommand)
                        {
                            incomingCmd.Execute();
                            break;
                        }
                        Scheduler.Add(incomingCmd);
                    }
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при выполнении команды: {ex.Message}");
                }
            }
        }

        internal void ExecuteHardStop()
        {
            if (Thread.CurrentThread != thread)
            {
                throw new InvalidOperationException("Команду остановки можно вызвать только внутри ServerThread");
            }
            runcontinue = false;
        }

        internal void ExecuteSoftStop()
        {
            if (Thread.CurrentThread != thread)
            {
                throw new InvalidOperationException("Команду остановки можно вызвать только внутри ServerThread");
            }
            queue.CompleteAdding();
        }

        public void Join()
        {
            thread.Join();
        }
    }

    public class HardStopCommand : ICommand
    {
        public readonly ServerThread _serverThread;

        public HardStopCommand(ServerThread serverThread)
        {
            _serverThread = serverThread;
        }

        public void Execute()
        {
            _serverThread.ExecuteHardStop();
        }
    }

    public class SoftStopCommand : ICommand
    {
        public readonly ServerThread _serverThread;

        public SoftStopCommand(ServerThread serverThread)
        {
            _serverThread = serverThread;
        }

        public void Execute()
        {
            _serverThread.ExecuteSoftStop();
        }
    }

    public class ImplementationBigCommand : ICommand
    {
        public int CountImplementation = 0;
        public int MaxcountImplementation { get; set; }

        public ImplementationBigCommand(int maxcountImplementation)
        {
            MaxcountImplementation = maxcountImplementation;
        }

        public virtual void Execute()
        {
            CountImplementation++;
        }
    }

    public class RoundRobinScheduler : IScheduler
    {
        public Queue<ICommand> QueueCommand = new Queue<ICommand>();

        public bool HasCommand()
        {
            if (QueueCommand.Count == 0) { return false; }
            return true;
        }

        public ICommand Select()
        {
            return QueueCommand.Dequeue();
        }

        public void Add(ICommand cmd)
        {
            QueueCommand.Enqueue(cmd);
        }
    }
}