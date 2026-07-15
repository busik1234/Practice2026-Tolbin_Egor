using Xunit;
using task14;
using System;
namespace task14tests
{
    using System;
    using Xunit;

    namespace task14.Tests
    {
        public class DefiniteIntegralTests
        {
            [Fact]
            public void Solve_LinearFunctionSymmetricInterval_ReturnsZero()
            {
                Func<double, double> X = x => x;

                double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);

                Assert.Equal(0, result, 1e-4);
            }

            [Fact]
            public void Solve_SineFunctionSymmetricInterval_ReturnsZero()
            {
                Func<double, double> SIN = x => Math.Sin(x);

                double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);

                Assert.Equal(0, result, 1e-4);
            }

            [Fact]
            public void Solve_LinearFunctionPositiveInterval_ReturnsCorrectValue()
            {
                Func<double, double> X = x => x;

                double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);

                Assert.Equal(12.5, result, 1e-5);
            }
        }
    }
}
