using System;
using System.Numerics;
 
namespace kursovaya.Methods.Equations
{
    public class Cubic
    {
        private readonly double a;
        private readonly double b;
        private readonly double c;
        private readonly double d;

        public Cubic(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        // f(x) = ax³ + bx² + cx + d
        public Complex Evaluate(Complex x)
        {
            Complex x2 = x * x;
            Complex x3 = x2 * x;
            return a * x3 + b * x2 + c * x + d;
        }

        // f'(x) = 3ax² + 2bx + c
        public Complex Derivative(Complex x)
        {
            Complex x2 = x * x;
            return 3 * a * x2 + 2 * b * x + c;
        }

        // Вещественная версия f(x) — для метода бисекции
        public double EvaluateReal(double x)
        {
            return a * x * x * x + b * x * x + c * x + d;
        }

        // Алгебраическое решение (формула Кардано)
        public Complex[] SolveAlgebraically()
        {
            if (Math.Abs(a) < 1e-12)
                throw new ArgumentException("Not a cubic equation (a == 0).");

            double p = (3 * a * c - b * b) / (3 * a * a);
            double q = (2 * b * b * b - 9 * a * b * c + 27 * a * a * d) / (27 * a * a * a);
            double offset = -b / (3 * a);

            Complex delta = Complex.Pow(q / 2.0, 2) + Complex.Pow(p / 3.0, 3);
            Complex sqrtDelta = Complex.Sqrt(delta);

            Complex u = CubeRoot(-q / 2.0 + sqrtDelta);
            Complex v = CubeRoot(-q / 2.0 - sqrtDelta);

            Complex omega = new Complex(-0.5, Math.Sqrt(3) / 2); // кубический корень из 1
            Complex x1 = u + v + offset;
            Complex x2 = u * omega + v * Complex.Conjugate(omega) + offset;
            Complex x3 = u * Complex.Conjugate(omega) + v * omega + offset;

            return new Complex[] { x1, x2, x3 };
        }

        // Метод Ньютона
        public Complex SolveWithNewton(double precision, Complex initialGuess, out int iterationsCount, int maxIterations = 1000)
        {
            return Methods.NewtonMethod(Evaluate, Derivative, initialGuess, precision, out iterationsCount, maxIterations);
        }

        // Метод бисекции (для действительных корней)
        public Complex SolveWithBisection(double left, double right, double precision, out int iterationsCount, int maxIterations = 1000)
        {
            return Methods.BisectionMethod(EvaluateReal, left, right, precision, out iterationsCount, maxIterations);
        }

        // Кубический корень комплексного числа
        private Complex CubeRoot(Complex z)
        {
            double r = Complex.Abs(z);
            double theta = Math.Atan2(z.Imaginary, z.Real);
            double rootR = Math.Pow(r, 1.0 / 3.0);
            double rootTheta = theta / 3.0;
            return new Complex(
                rootR * Math.Cos(rootTheta),
                rootR * Math.Sin(rootTheta)
            );
        }
    }
}
