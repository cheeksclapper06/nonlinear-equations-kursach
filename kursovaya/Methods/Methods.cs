using System.Numerics;
using System.Windows;
using static System.Windows.MessageBox;
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
            Complex z = initialGuess;
            iterations = 0;
            try 
            { 
                while (iterations < maxIterations)
                {
                    Complex fz = function(z);
                    Complex dfz = derivative(z);

                    if (Complex.Abs(dfz) < 1e-12)
                    {
                        throw new DivideByZeroException("Derivative is too small.");
                    }

                    Complex zNew = z - fz / dfz;

                    if (Complex.Abs(zNew - z) < precision)
                    {
                        return zNew;
                    }

                    z = zNew;
                    iterations++;
                }

                throw new Exception("Newton method did not converge.");
            }
            catch (Exception ex)
            {
                Show(ex.Message, "Newton method warning", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        
        public static Complex BisectionMethod(
            Func<double, double> function,
            double left,
            double right,
            double precision,
            out int iterations,
            int maxIterations = 1000)
        {
            double mid = 0;
            iterations = 0;

            try
            {
                if (function(left) * function(right) > 0)
                {
                    throw new ArgumentException("Function must have opposite signs at the endpoints.");
                }
            
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
            catch(Exception ex)
            {
                Show(ex.Message, "Bisection method warning", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
           
        }
    }
}