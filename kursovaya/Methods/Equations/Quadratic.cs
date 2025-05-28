using System.Numerics;
using System.Diagnostics;
namespace kursovaya.Methods.Equations
{
    public class Quadratic : MainWindow.IPolynomial
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

        public Complex[] SolveAlgebraically(out TimeSpan elapsed)
        { 
            var time = Stopwatch.StartNew();
           
            Complex discriminant = double.Pow(b, 2) - 4 * a * c; 
            Complex sqrtD = Complex.Sqrt(discriminant);

            Complex z1 = (-b + sqrtD) / (2 * a);
            Complex z2 = (-b - sqrtD) / (2 * a);
            
            time.Stop();
            elapsed = time.Elapsed;
            return [z1, z2];
        }

        public Complex SolveWithNewton(double precision, Complex initialGuess, out int iterationsCount, out TimeSpan elapsed, int maxIterations = 1000)
        {
            var time = Stopwatch.StartNew();
            Complex root = Methods.NewtonMethod(Evaluate, Derivative, initialGuess, precision, out iterationsCount, maxIterations);
            time.Stop();
            elapsed = time.Elapsed;
            return root;
        }

        public Complex SolveWithBisection(double left, double right, double precision, out int iterationsCount, out TimeSpan elapsed, int maxIterations = 1000)
        {
            var time = Stopwatch.StartNew();
            Complex root = Methods.BisectionMethod(EvaluateReal, left, right, precision, out iterationsCount, maxIterations);
            time.Stop();
            elapsed = time.Elapsed;
            return root;
        }
        
    }
}