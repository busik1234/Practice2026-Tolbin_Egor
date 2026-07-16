using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task14
{
    public class ThreadMyClass
    {
        public double StartValue { get; set; }
        public double FinishValue { get; set; }
        public Func<double, double> Func { get; set; }
        public double Step { get; set; }

    }
}
