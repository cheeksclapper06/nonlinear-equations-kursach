using System.Numerics;

namespace kursovaya.Methods
{
    public static class Methods
    {
        public static Complex NewtonMethod(
            Func<Complex, Complex> function,
            Func<Complex, Complex> derivative,
            Complex initialGuess,
            double precision,
            out int iterations,
            int maxIterations = 1000)
        {
            Complex x = initialGuess;
            iterations = 0;

            while (iterations < maxIterations)
            {
                Complex fx = function(x);
                Complex dfx = derivative(x);

                if (Complex.Abs(dfx) < 1e-12)
                {
                    throw new DivideByZeroException("Derivative is too small.");
                }

                Complex xNew = x - fx / dfx;

                if (Complex.Abs(xNew - x) < precision)
                {
                    return xNew;
                }

                x = xNew;
                iterations++;
            }

            throw new Exception("Newton method did not converge.");
        }

        public static Complex BisectionMethod(
            Func<double, double> function,
            double left,
            double right,
            double precision,
            out int iterations,
            int maxIterations = 1000)
        {
            if (function(left) * function(right) > 0)
            {
                throw new ArgumentException("Function must have opposite signs at the endpoints.");
            }

            double mid = 0;
            iterations = 0;
            while ((right - left) / 2.0 > precision && iterations < maxIterations)
            {
                mid = (left + right) / 2.0;
                double fMid = function(mid);

                if (Math.Abs(fMid) < precision)
                {
                    break;
                }
                
                if (function(left) * fMid < 0)
                {
                    right = mid;
                }
                else
                {
                    left = mid;
                }

                iterations++;
            }

            return new Complex(mid, 0);
        }
    }
}