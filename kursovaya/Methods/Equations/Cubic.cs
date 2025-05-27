using System;
using System.Numerics;
using System.Windows;
using static System.Windows.MessageBox;
 
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

        // f(z) = az³ + bz² + cz + d
        public Complex Evaluate(Complex z)
        {
            Complex z2 = z * z;
            Complex z3 = z2 * z;
            return a * z3 + b * z2 + c * z + d;
        }

        // f'(x) = 3ax² + 2bx + c
        public Complex Derivative(Complex z)
        {
            Complex z2 = z * z;
            return 3 * a * z2 + 2 * b * z + c;
        }

        // Вещественная версия f(x) — для метода бисекции
        public double EvaluateReal(double z)
        {
            return a * double.Pow(z, 3) + b * double.Pow(z, 2) + c * z + d;
        }

        // Алгебраическое решение (формула Кардано)
        public Complex[] SolveAlgebraically()
        {
            try
            {
                if (Math.Abs(a) == 0)
                {
                    throw new ArgumentException("Not a cubic equation (a == 0).");
                }
            }
            catch (Exception ex)
            {
                Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            double p = (3 * a * c - double.Pow(b, 2))/ (3 * double.Pow(a, 2));
            double q = (2 * double.Pow(b, 3) - 9 * a * b * c + 27 * double.Pow(a, 2) * d) / (27 * double.Pow(a, 3));
            double offset = -b / (3 * a);

            Complex delta = Complex.Pow(q / 2.0, 2) + Complex.Pow(p / 3.0, 3);
            Complex sqrtDelta = Complex.Sqrt(delta);

            Complex u = CubeRoot(-q / 2.0 + sqrtDelta);
            Complex v = CubeRoot(-q / 2.0 - sqrtDelta);

            Complex omega = new Complex(-0.5, Math.Sqrt(3) / 2); // кубический корень из 1
            Complex z1 = u + v + offset;
            Complex z2 = u * omega + v * Complex.Conjugate(omega) + offset;
            Complex z3 = u * Complex.Conjugate(omega) + v * omega + offset;

            return new Complex[] { z1, z2, z3 };
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
