using System;
using System.Numerics;

namespace kursovaya.Methods.Equations
{
    public class Biquadratic
    {
        private readonly double a;
        private readonly double b;
        private readonly double c;

        public Biquadratic(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        // f(x) = ax⁴ + bx² + c
        public Complex Evaluate(Complex x)
        {
            Complex x2 = x * x;
            Complex x4 = x2 * x2;
            return a * x4 + b * x2 + c;
        }

        // f'(x) = 4ax³ + 2bx
        public Complex Derivative(Complex x)
        {
            Complex x2 = x * x;
            Complex x3 = x2 * x;
            return 4 * a * x3 + 2 * b * x;
        }

        // f(x) as a real function — for Bisection Method
        public double EvaluateReal(double x)
        {
            return a * Math.Pow(x, 4) + b * Math.Pow(x, 2) + c;
        }

        // Solve algebraically in complex field
        public Complex[] SolveAlgebraically()
        {
            // Substitute y = x^2 => ay² + by + c = 0
            Complex A = a;
            Complex B = b;
            Complex C = c;

            Complex discriminant = B * B - 4 * A * C;
            Complex sqrtD = Complex.Sqrt(discriminant);

            Complex y1 = (-B + sqrtD) / (2 * A);
            Complex y2 = (-B - sqrtD) / (2 * A);

            Complex[] roots = new Complex[4];
            roots[0] = Complex.Sqrt(y1);
            roots[1] = -roots[0];
            roots[2] = Complex.Sqrt(y2);
            roots[3] = -roots[2];

            return roots;
        }

        // Newton's method
        public Complex SolveWithNewton(double precision, Complex initialGuess, out int iterationsCount, int maxIterations = 1000)
        {
            return Methods.NewtonMethod(
                Evaluate,
                Derivative,
                initialGuess,
                precision,
                out iterationsCount,
                maxIterations
            );
        }

        // Bisection method (real roots only)
        public Complex SolveWithBisection(double left, double right, double precision, out int iterationsCount, int maxIterations = 1000)
        {
            return Methods.BisectionMethod(
                EvaluateReal,
                left,
                right,
                precision,
                out iterationsCount,
                maxIterations
            );
        }
    }
}
