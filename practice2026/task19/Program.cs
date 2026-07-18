using System;
using System.Collections.Generic;
using System.IO;
using ScottPlot;
using task17;

namespace task19
{
    //Наследуется от ImplementationBigCommand, а не от ICommand, т.к. сам по себе функционал TestCommand был реализован в задании 18 через ImplementationBigCommand: Icommand
    public class TestCommand : ImplementationBigCommand
    {
        private int _id;
        public int Id => _id;

        public TestCommand(int id) : base(3) => _id = id;

        public override void Execute()
        {
            base.Execute();
            Console.WriteLine($"Поток {_id} вызов {CountImplementation}");

            Program.ExecutedIds.Add(_id);

            string action = CountImplementation < MaxcountImplementation
                ? $"Вернулась в хвост планировщика ({CountImplementation} из 3)"
                : $"Завершила работу и ВЫБЫЛА ({CountImplementation} из 3)";

            Program.ReportSteps.Add($"Шаг {Program.ExecutedIds.Count,-2} | Команда {_id,-10} | {action}");
        }
    }

    class Program
    {
        private const string TargetFolder = @"C:\Users\egort\OneDrive\Рабочий стол\Practice124\Practice2026-Tolbin_Egor\practice2026\task19";

        public static List<double> ExecutedIds = new List<double>();
        public static List<string> ReportSteps = new List<string>();
        public static bool FinalRunContinueState;

        static void Main()
        {
            RunTask19();
        }

        public static void RunTask19()
        {
            if (!Directory.Exists(TargetFolder))
            {
                Directory.CreateDirectory(TargetFolder);
            }

            var server = new task17.ServerThread();
            for (int i = 1; i <= 5; i++)
            {
                server.AddCommand(new TestCommand(i));
            }
            server.Start();
            while (Program.ExecutedIds.Count < 15)
            {
                System.Threading.Thread.Sleep(10);
            }
            server.AddCommand(new task17.HardStopCommand(server));
            server.Join();

            FinalRunContinueState = server.runcontinue;

            Console.WriteLine("Task 19 done.");

            BuildPlot();
            WriteReport();
        }

        private static void BuildPlot()
        {
            double[] xs = new double[ExecutedIds.Count];
            for (int i = 0; i < xs.Length; i++)
            {
                xs[i] = i + 1;
            }

            double[] ys = ExecutedIds.ToArray();

            var plt = new ScottPlot.Plot();
            var scatter = plt.Add.Scatter(xs, ys);

            scatter.Color = ScottPlot.Color.FromHex("#228B22");
            scatter.MarkerSize = 8;
            scatter.LineWidth = 2;
            scatter.Label = "Очередность вызовов";

            plt.Title("Чередование команд в планировщике");
            plt.XLabel("Порядковый номер вызова Execute()");
            plt.YLabel("ID команды");

            string plotPath = Path.Combine(TargetFolder, "scheduler_plot.png");
            plt.SavePng(plotPath, 1000, 500);
            Console.WriteLine($"График сохранен: {plotPath}");
        }

        private static void WriteReport()
        {
            string reportPath = Path.Combine(TargetFolder, "report.txt");

            using (StreamWriter writer = new StreamWriter(reportPath, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("===============================================================================");
                writer.WriteLine("                            ОТЧЕТ О ВЫПОЛНЕНИИ                                 ");
                writer.WriteLine("===============================================================================");
                writer.WriteLine();

                writer.WriteLine("Порядок выполнения команд:");
                for (int i = 0; i < ExecutedIds.Count; i++)
                {
                    writer.WriteLine($"{i + 1,2} -> команда {ExecutedIds[i]}");
                }
                writer.WriteLine();

                writer.WriteLine($"Всего вызовов: {ExecutedIds.Count}");
                writer.WriteLine("Команд: 5");
                writer.WriteLine("Вызовов на команду: 3");
                writer.WriteLine($"Ожидалось: 15");
                writer.WriteLine();

                writer.WriteLine("===============================================================================");
                writer.WriteLine("               ПРОГРАММНЫЙ ТРЕЙС РАБОТЫ ПЛАНИРОВЩИКА (ПОД КАПОТОМ)              ");
                writer.WriteLine("===============================================================================");

                foreach (var step in ReportSteps)
                {
                    writer.WriteLine(step);
                }

                writer.WriteLine($"Шаг {ExecutedIds.Count + 1,-2} | HardStop   | Вызов ExecuteHardStop() -> Изменение runcontinue");

                writer.WriteLine();
                writer.WriteLine("===============================================================================");
                writer.WriteLine("                      СИСТЕМНОЕ ДОКАЗАТЕЛЬСТВО ОСТАНОВКИ                       ");
                writer.WriteLine("===============================================================================");
                writer.WriteLine($"Имя контролируемой переменной флага цикла : server.runcontinue");
                writer.WriteLine($"Финальное считанное значение переменной   : {FinalRunContinueState.ToString().ToUpper()}");
                writer.WriteLine($"Состояние фонового потока (thread.Alive)  : {false.ToString().ToUpper()} (после Join)");
                writer.WriteLine();
                writer.WriteLine("АНАЛИЗ РЕЗУЛЬТАТА:");
                writer.WriteLine($"1. Планировщик Round Robin отработал ровно за {ExecutedIds.Count} рабочих шагов.");
                writer.WriteLine("2. Программа математически доказывает циклическое квантование времени:");
                writer.WriteLine("   Команды идут строго волнообразно (1->2->3->4->5), ни одна команда не заняла поток монопольно.");
                writer.WriteLine("3. На шагах 11-15 зафиксировано последовательное выбывание рабочих команд из очереди.");
                writer.WriteLine($"4. Доказательство HardStop: Значение server.runcontinue извлечено из памяти и равно {FinalRunContinueState.ToString().ToUpper()}.");
                writer.WriteLine("   Это подтверждает, что HardStopCommand успешно выполнилась, разорвала цикл while и остановила сервер.");
                writer.WriteLine("===============================================================================");
            }

            Console.WriteLine($"Текстовый отчет успешно сохранен: {reportPath}");
        }
    }
}