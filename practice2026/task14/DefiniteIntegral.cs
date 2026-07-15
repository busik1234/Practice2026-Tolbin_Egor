using System;
using System.Threading;
namespace task14
{
    public class DefiniteIntegral
    {
        private static double integralsum = 0;
        private static Barrier barrier;

        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
        {
            integralsum = 0;
            double h = (b - a) / threadsnumber;
            barrier = new Barrier(threadsnumber + 1);
            for (int i = 0; i < threadsnumber; i++)
            {
                double startvalue = a + i * h;
                double finishvalue = 0;

                if (i == threadsnumber - 1) { finishvalue = b; }
                else { finishvalue = a + (i + 1) * h; }

                ThreadMyClass datamythread = new ThreadMyClass
                {
                    StartValue = startvalue,
                    FinishValue = finishvalue,
                    Func = function,
                    Step = step,
                };
                Thread realthread = new Thread(IntegralThisSum);
                realthread.Start(datamythread);
            }

            barrier.SignalAndWait();
            return integralsum;
        }
        public static void IntegralThisSum(object obj)
        {
            ThreadMyClass datamythread = (ThreadMyClass)obj;
            double localintergralsum = 0;

            int maxi = (int)Math.Ceiling((datamythread.FinishValue - datamythread.StartValue) / datamythread.Step);
            for (int i = 0; i < maxi; i++)
            {
                double x1 = datamythread.StartValue + i * datamythread.Step;
                double x2 = datamythread.StartValue + (i + 1) * datamythread.Step;
                if (x2 > datamythread.FinishValue)
                {
                    x2 = datamythread.FinishValue;
                }

                double y1 = datamythread.Func(x1);
                double y2 = datamythread.Func(x2);

                double localintegralvalue = ((y1 + y2) / 2.0) * (x2 - x1);
                localintergralsum += localintegralvalue;
            }
            AddToTotalSum(localintergralsum);
            barrier.SignalAndWait();
        }
        private static void AddToTotalSum(double value)
        {
            double initialValue, computedValue;
            do
            {
                initialValue = integralsum;
                computedValue = initialValue + value;
            }
            while (Interlocked.CompareExchange(ref integralsum, computedValue, initialValue) != initialValue);
        }
    }
}