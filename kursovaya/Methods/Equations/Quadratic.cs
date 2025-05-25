using System;
using System.Numerics;

namespace kursovaya.Methods.Equations
{
    public class Quadratic
    {
        private readonly double a;
        private readonly double b;
        private readonly double c;

        public Quadratic(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public Complex Evaluate(Complex x)
        {
            return a * x * x + b * x + c;
        }

        public Complex Derivative(Complex x)
        {
            return 2 * a * x + b;
        }

        public double EvaluateReal(double x)
        {
            return a * x * x + b * x + c;
        }

        public Complex[] SolveAlgebraically()
        {
            Complex discriminant = b * b - 4 * a * c;
            Complex sqrtD = Complex.Sqrt(discriminant);

            Complex x1 = (-b + sqrtD) / (2 * a);
            Complex x2 = (-b - sqrtD) / (2 * a);

            return new[] { x1, x2 };
        }

        public Complex SolveWithNewton(double precision, Complex initialGuess, out int iterationsCount, int maxIterations = 1000)
        {
            return Methods.NewtonMethod(Evaluate, Derivative, initialGuess, precision, out iterationsCount, maxIterations);
        }

        public Complex SolveWithBisection(double left, double right, double precision, out int iterationsCount, int maxIterations = 1000)
        {
            return Methods.BisectionMethod(EvaluateReal, left, right, precision, out iterationsCount, maxIterations);
        }
    }
}