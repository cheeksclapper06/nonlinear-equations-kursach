﻿using System;
using System.Numerics;
using System.Diagnostics;

namespace kursovaya.Methods.Equations
{
    public class Biquadratic : MainWindow.IPolynomial
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

        public Complex Evaluate(Complex z)
        {
            Complex z2 = z * z;
            Complex z4 = z2 * z2;
            return a * z4 + b * z2 + c;
        }

        public Complex Derivative(Complex z)
        {
            Complex z2 = z * z;
            Complex z3 = z2 * z;
            return 4 * a * z3 + 2 * b * z;
        }

        public double EvaluateReal(double z)
        {
            return a * Math.Pow(z, 4) + b * Math.Pow(z, 2) + c;
        }

        public Complex[] SolveAlgebraically(out TimeSpan elapsed)
        {
            var time = Stopwatch.StartNew();
            
            Complex A = a;
            Complex B = b;
            Complex C = c;

            Complex discriminant = B * B - 4 * A * C;
            Complex sqrtD = Complex.Sqrt(discriminant);

            Complex t1 = (-B + sqrtD) / (2 * A);
            Complex t2 = (-B - sqrtD) / (2 * A);

            Complex[] z = new Complex[4];
            z[0] = Complex.Sqrt(t1);
            z[1] = -z[0];
            z[2] = Complex.Sqrt(t2);
            z[3] = -z[2];

            time.Stop();
            elapsed = time.Elapsed;
            
            return z;
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
