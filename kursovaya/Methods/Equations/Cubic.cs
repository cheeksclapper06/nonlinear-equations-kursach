using System;
using System.Numerics;
using System.Diagnostics;

namespace kursovaya.Methods.Equations;

public class Cubic : MainWindow.IPolynomial
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

    public Complex Evaluate(Complex z)
    {
        Complex z2 = z * z;
        Complex z3 = z2 * z;
        return a * z3 + b * z2 + c * z + d;
    }

    public Complex Derivative(Complex z)
    {
        Complex z2 = z * z;
        return 3 * a * z2 + 2 * b * z + c;
    }

    public double EvaluateReal(double z)
    {
        return a * double.Pow(z, 3) + b * double.Pow(z, 2) + c * z + d;
    }

    public Complex[] SolveAlgebraically(out TimeSpan elapsed)
    {
        var time = Stopwatch.StartNew();
        
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

        time.Stop();
        elapsed = time.Elapsed;
        
        return [z1, z2, z3];
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