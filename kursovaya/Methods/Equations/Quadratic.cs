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

        public Complex Evaluate(Complex z)
        {
            return a * Complex.Pow(z, 2) + b * z + c;
        }

        public Complex Derivative(Complex z)
        {
            return 2 * a * z + b;
        }

        public double EvaluateReal(double z)
        {
            return a * double.Pow(z, 2) + b * z + c;
        }

        public Complex[] SolveAlgebraically()
        {
            Complex discriminant = double.Pow(b, 2) - 4 * a * c;
            Complex sqrtD = Complex.Sqrt(discriminant);

            Complex z1 = (-b + sqrtD) / (2 * a);
            Complex z2 = (-b - sqrtD) / (2 * a);

            return new[] { z1, z2 };
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