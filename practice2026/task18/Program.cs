using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using task17;
using ScottPlot;

namespace task18
{
    public class MyTaskCommand : ImplementationBigCommand
    {
        public int Step;

        public MyTaskCommand(int maxOps, int step) : base((maxOps + step - 1) / step)
        {
            Step = step;
        }

        public override void Execute()
        {
            CountImplementation++;

            for (int i = 0; i < Step; i++)
            {
                Thread.SpinWait(10);
            }
        }
    }

    class Program
    {
        static string folder = @"C:\Users\egort\OneDrive\Рабочий стол\Practice124\Practice2026-Tolbin_Egor\practice2026\task18";

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            }
            catch { }

            {
                ServerThread warmupServer = new ServerThread();
                warmupServer.AddCommand(new MyTaskCommand(1000, 10));
                warmupServer.AddCommand(new SoftStopCommand(warmupServer));
                warmupServer.Start();
                warmupServer.Join();
            }

            int totalOps = 1_000_000;
            int numTasks = 4;
            int repeats = 5;

            double[] quantumSizes = { 10, 50, 100, 500, 1000, 5000, 20000 };
            double[] executionTimes = new double[quantumSizes.Length];

            Console.WriteLine("Запуск калиброванного теста стабильности...");

            for (int i = 0; i < quantumSizes.Length; i++)
            {
                int currentQuantum = (int)quantumSizes[i];
                Console.WriteLine($"Тест {i + 1}/{quantumSizes.Length}: Квант = {currentQuantum}...");

                var samples = new List<double>();

                for (int r = 0; r < repeats; r++)
                {
                    Stopwatch sw = new Stopwatch();
                    ServerThread server = new ServerThread();

                    for (int t = 0; t < numTasks; t++)
                    {
                        server.AddCommand(new MyTaskCommand(totalOps, currentQuantum));
                    }
                    server.AddCommand(new SoftStopCommand(server));

                    sw.Start();
                    server.Start();
                    server.Join();
                    sw.Stop();

                    samples.Add(sw.Elapsed.TotalMilliseconds);
                }

                executionTimes[i] = Median(samples);
                Console.WriteLine($"   Замеры: {string.Join(", ", samples.Select(s => s.ToString("F1")))} мс");
                Console.WriteLine($"   Медиана: {executionTimes[i]:F1} мс");
            }

            MakeReport(quantumSizes, executionTimes, totalOps, numTasks);
            DrawGraph(quantumSizes, executionTimes);

            Console.WriteLine($"\nГотово! Результаты здесь:\n{folder}");
        }

        static double Median(List<double> values)
        {
            var sorted = values.OrderBy(v => v).ToList();
            int mid = sorted.Count / 2;
            return sorted.Count % 2 == 0
                ? (sorted[mid - 1] + sorted[mid]) / 2.0
                : sorted[mid];
        }

        static void MakeReport(double[] quantums, double[] times, int totalOps, int numTasks)
        {
            string path = Path.Combine(folder, "report_task18.txt");
            int bestIndex = 0;
            for (int i = 1; i < times.Length; i++)
            {
                if (times[i] < times[bestIndex]) bestIndex = i;
            }

            using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("ОТЧЕТ: Тестирование планировщика методом калиброванных задержек");
                writer.WriteLine("=================================================================");
                writer.WriteLine();
                writer.WriteLine($"Параметры: {numTasks} задач по {totalOps:N0} квантованных шагов ожидания.");
                writer.WriteLine();
                for (int i = 0; i < quantums.Length; i++)
                {
                    writer.WriteLine($"Квант {quantums[i],8:N0} -> Время: {times[i]:F1} мс");
                }
            }
        }


        static void DrawGraph(double[] xs, double[] ys)
        {
            string plotPath = Path.Combine(folder, "efficiency_chart.png");
            try
            {
                double[] logXs = xs.Select(x => Math.Log10(x)).ToArray();
                var plt = new ScottPlot.Plot();
                var scatter = plt.Add.Scatter(logXs, ys);
                scatter.LineWidth = 3;
                scatter.MarkerSize = 10;
                scatter.Color = ScottPlot.Color.FromHex("#E31A1C");
                plt.Title("Зависимость накладных расходов от размера кванта");
                plt.XLabel("Размер кванта (исходные единицы)");
                plt.YLabel("Медианное время выполнения (мс)");

                ScottPlot.Tick[] customTicks = new ScottPlot.Tick[xs.Length];
                for (int i = 0; i < xs.Length; i++)
                {
                    customTicks[i] = new ScottPlot.Tick(logXs[i], xs[i].ToString("N0"));
                }
                plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(customTicks);

                plt.SavePng(plotPath, 900, 500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка построения графика: {ex.Message}");
            }
        }
    }
}