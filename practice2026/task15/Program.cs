using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using task14;
using ScottPlot;

namespace task15
{
    class Program
    {
        private const int MaxThreadsToTest = 16;
        private const int RunsCount = 5;

        public static void Main(string[] args)
        {
            double a = -100;
            double b = 100;
            Func<double, double> sinFunc = Math.Sin;

            DefiniteIntegral.Solve(a, b, sinFunc, 1e-2, 1);

            double optimalStep = FindOptimalStep(a, b, sinFunc);

            var threadResults = FindOptimalThreads(a, b, sinFunc, optimalStep);
            int optimalThreads = threadResults.OptimalThreads;
            double avgMultiTime = threadResults.BestTime;

            double avgSingleTime = MeasureSingleThread(a, b, sinFunc, optimalStep);

            ComparePerformance(avgSingleTime, avgMultiTime, out double speedupPercent);

            SaveReport(a, b, optimalStep, avgSingleTime, optimalThreads, avgMultiTime, speedupPercent, threadResults.RawData);
        }

        // 1. Поиск оптимального шага по теоретической формуле погрешности метода трапеций
        public static double FindOptimalStep(double a, double b, Func<double, double> function)
        {
            double[] candidateSteps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
            const double targetAccuracy = 1e-4;
            const double maxSecondDerivative = 1.0;

            foreach (var step in candidateSteps)
            {
                double errorBound = (b - a) * step * step / 12.0 * maxSecondDerivative;
                double result = DefiniteIntegral.Solve(a, b, function, step, 1);

                if (errorBound < targetAccuracy)
                {
                    return step;
                }
            }

            return candidateSteps[candidateSteps.Length - 1];
        }

        public class ThreadResearchResult
        {
            public int OptimalThreads { get; set; }
            public double BestTime { get; set; }
            public Dictionary<int, double> RawData { get; set; }
        }

        //2. Замеры времени для разного числа потоков
        public static ThreadResearchResult FindOptimalThreads(double a, double b, Func<double, double> function, double step)
        {
            var rawData = new Dictionary<int, double>();

            for (int threads = 1; threads <= MaxThreadsToTest; threads++)
            {
                var times = new List<double>();
                for (int run = 0; run < RunsCount; run++)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    DefiniteIntegral.Solve(a, b, function, step, threads);
                    sw.Stop();
                    times.Add(sw.Elapsed.TotalMilliseconds);
                }
                double avgTime = times.Skip(1).Average();
                rawData.Add(threads, avgTime);
            }

            var bestConfig = rawData.OrderBy(kvp => kvp.Value).First();

            return new ThreadResearchResult
            {
                OptimalThreads = bestConfig.Key,
                BestTime = bestConfig.Value,
                RawData = rawData
            };
        }

        //3. Однопоточный расчет площади без использования класса Thread
        public static double MeasureSingleThread(double a, double b, Func<double, double> function, double step)
        {
            var times = new List<double>();

            for (int run = 0; run < RunsCount; run++)
            {
                Stopwatch sw = Stopwatch.StartNew();

                double sum = 0;
                int maxi = (int)Math.Ceiling((b - a) / step);

                for (int i = 0; i < maxi; i++)
                {
                    double x1 = a + i * step;
                    double x2 = a + (i + 1) * step;

                    if (x2 > b)
                    {
                        x2 = b;
                    }

                    double y1 = function(x1);
                    double y2 = function(x2);

                    double localintegralvalue = ((y1 + y2) / 2.0) * (x2 - x1);
                    sum += localintegralvalue;
                }

                sw.Stop();
                times.Add(sw.Elapsed.TotalMilliseconds);
            }

            return times.Skip(1).Average();
        }

        // 4. Проверка прироста производительности
        public static bool ComparePerformance(double singleThreadTime, double multiThreadTime, out double speedupPercent)
        {
            speedupPercent = ((singleThreadTime - multiThreadTime) / singleThreadTime) * 100;
            return speedupPercent > 15.0;
        }

        // 5. Сохранение отчета и графика
        private static void SaveReport(
            double a, double b, double step, double singleTime,
            int optThreads, double multiTime, double diffPercent, Dictionary<int, double> threadData)
        {
            string reportPath = @"C:\Users\egort\OneDrive\Рабочий стол\Practice124\Practice2026-Tolbin_Egor\practice2026\task15\report.txt";
            string plotPath = @"C:\Users\egort\OneDrive\Рабочий стол\Practice124\Practice2026-Tolbin_Egor\practice2026\task15\analysgraph.png";

            double[] xs = threadData.Keys.Select(k => (double)k).ToArray();
            double[] ys = threadData.Values.ToArray();

            var plt = new ScottPlot.Plot();
            plt.Add.Scatter(xs, ys);
            plt.Title("Зависимость времени расчета от количества потоков");
            plt.XLabel("Количество потоков (шт)");
            plt.YLabel("Время выполнения (мс)");
            plt.SavePng(plotPath, 600, 400);

            using (StreamWriter writer = new StreamWriter(reportPath))
            {
                writer.WriteLine("=========================================================================");
                writer.WriteLine("          ОТЧЕТ ПО ЛАБОРАТОРНОЙ РАБОТЕ: МНОГОПОТОЧНОЕ ИНТЕГРИРОВАНИЕ     ");
                writer.WriteLine("=========================================================================");
                writer.WriteLine();
                writer.WriteLine("Заданная точность: 0,0001");
                writer.WriteLine($"Интервал интегрирования: [{a}, {b}]");
                writer.WriteLine("Интегрируемая функция: sin(x)");
                writer.WriteLine();

                writer.WriteLine("1. ОБОСНОВАНИЕ ВЫБОРА ШАГА");
                writer.WriteLine("-------------------------------------------------------------------------");
                writer.WriteLine("Теоретическая оценка погрешности формулы трапеций R(f) <= (b-a) * (h^2)/12 * max|f''(x)|:");

                double[] candidateSteps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
                foreach (var s in candidateSteps)
                {
                    double errorBound = (b - a) * s * s / 12.0 * 1.0;
                    double result = DefiniteIntegral.Solve(a, b, Math.Sin, s, 1);
                    writer.WriteLine($"  - Шаг {s:0.0E+00}: результат = {result:E6}, теор. погрешность = {errorBound:E6}");
                }

                writer.WriteLine();
                writer.WriteLine($"Выбираем шаг h = {step:0.0E+00}, так как он является оптимальным: гарантирует выполнение");
                writer.WriteLine("заданной точности 1e-4 и при этом не перегружает вычисления (в отличие от шагов 1e-4 и меньше).");
                writer.WriteLine();

                writer.WriteLine("2. РЕЗУЛЬТАТЫ ЗАМЕРОВ ДЛЯ РАЗНОГО КОЛИЧЕСТВА ПОТОКОВ");
                writer.WriteLine("-------------------------------------------------------------------------");
                writer.WriteLine("Результаты замеров среднего времени работы (первый 'холодный' запуск отброшен):");
                foreach (var kvp in threadData)
                {
                    writer.WriteLine($"  - Потоков: {kvp.Key,-2} | Время: {kvp.Value:F4} мс");
                }
                writer.WriteLine();
                writer.WriteLine($"Выбираем оптимальное число потоков: {optThreads}, так как у него минимальная скорость ({multiTime:F4} мс).");
                writer.WriteLine();

                writer.WriteLine("3. ЗАМЕР РАБОТЫ БЕЗ ПОТОКОВ (ОДНОПОТОК)");
                writer.WriteLine("-------------------------------------------------------------------------");
                writer.WriteLine($"Время работы в 1 поток: {singleTime:F4} мс");
                writer.WriteLine("(Был написан эквивалентный однопоточный метод для чистоты эксперимента без накладных расходов на потоки).");
                writer.WriteLine();

                writer.WriteLine("4. СРАВНЕНИЕ ПРОИЗВОДИТЕЛЬНОСТИ И КРИТЕРИЙ ЭФФЕКТИВНОСТИ");
                writer.WriteLine("-------------------------------------------------------------------------");
                writer.WriteLine($"Время однопоточного расчета: {singleTime:F4} мс");
                writer.WriteLine($"Оптимальное время многопоточного расчета ({optThreads} пот.): {multiTime:F4} мс");
                writer.WriteLine($"Прирост производительности программы составил: {diffPercent:F2}%");
                writer.WriteLine();

                writer.WriteLine("Вывод:");
                if (diffPercent > 15.0)
                {
                    writer.WriteLine($"Многопоточная версия работает быстрее однопоточной на {diffPercent:F2}%.");
                    writer.WriteLine("Результат удовлетворяет критерию эффективности (>15%).");
                    writer.WriteLine("Подробный график зависимости времени от числа потоков сохранен в analysgraph.png.");
                }
                else
                {
                    writer.WriteLine($"Многопоточная версия работает быстрее однопоточной на {diffPercent:F2}%.");
                    writer.WriteLine("Результат НЕ удовлетворяет критерию эффективности (>15%), требуется оптимизация.");
                }
                writer.WriteLine("=========================================================================");
            }
        }
    }
}