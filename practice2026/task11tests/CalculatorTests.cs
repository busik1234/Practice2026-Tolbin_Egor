using task11;

namespace task11tests
{
    public class CalculatorTests
    {
        private const string TeacherCode = @"
            public class Calculator
            {
                public int Add(int a,int b) => a + b;
                public int Minus(int a, int b) => a - b;
                public int Mul(int a, int b) => a * b;
                public int Div(int a, int b) => a / b;
            }";

        [Fact]
        public void CreateCalculator_ShouldCompileAndReturnInstance()
        {
            ICalculator calc = CalculatorGeneration.CreateCalculatorFromString(TeacherCode);

            Assert.NotNull(calc);
        }

        [Fact]
        public void Calculator_ShouldPerformArithmeticOperations()
        {
            ICalculator calc = CalculatorGeneration.CreateCalculatorFromString(TeacherCode);

            Assert.Equal(13, calc.Add(10, 3));
            Assert.Equal(5, calc.Minus(10, 5));
            Assert.Equal(81, calc.Mul(9, 9));
            Assert.Equal(20, calc.Div(100, 5));
        }
        [Fact]
        public void Calculator_ShouldPerformArithmeticOperationsWithNegativeNumbers()
        {
            ICalculator calc = CalculatorGeneration.CreateCalculatorFromString(TeacherCode);

            Assert.Equal(-4, calc.Add(-6, 2));
            Assert.Equal(-8, calc.Minus(-6, 2));
            Assert.Equal(-12, calc.Mul(-6, 2));
            Assert.Equal(-3, calc.Div(-6, 2));
        }
        [Fact]
        public void Calculator_ShouldThrowException_WhenDividingByZero()
        {
            ICalculator calc = CalculatorGeneration.CreateCalculatorFromString(TeacherCode);
            Assert.Throws<DivideByZeroException>(() => calc.Div(10, 0));
        }
    }
}
