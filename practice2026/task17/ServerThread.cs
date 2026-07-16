using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17
{
    public interface ICommand
    {
        void Execute();
    }

    public class ServerThread
    {
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
                    ICommand command = queue.Take();
                    command.Execute();
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
            VerifyIsCurrentThread();
            runcontinue = false;
        }

        internal void ExecuteSoftStop()
        {
            VerifyIsCurrentThread();
            queue.CompleteAdding();
        }

        private void VerifyIsCurrentThread()
        {
            if (Thread.CurrentThread != thread)
            {
                throw new InvalidOperationException("Команду остановки можно вызвать только внутри ServerThread!");
            }
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
}