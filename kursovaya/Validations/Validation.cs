using System;
using System.Globalization; // Для CultureInfo.InvariantCulture, NumberStyles
using System.Numerics;
using System.Runtime.InteropServices; // Для Complex
using System.Windows.Controls;
using System.Windows;
using static System.Windows.MessageBox; // Для TextBox

namespace kursovaya.Validation
{

    public static class Validate
    {
        public static bool TryParse4Coeffs(TextBox t1, TextBox t2, TextBox t3, TextBox t4, out double a, out double b, out double c, out double d)
        {
            a = 0; b = 0; c = 0; d = 0;
            double maxCoeffValue = 10000;
            double minCoeffValue = -10000;

            bool isValid = double.TryParse(t1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out a) &&
                           double.TryParse(t2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out b) &&
                           double.TryParse(t3.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out c) &&
                           double.TryParse(t4.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out d);
            if (a == 0)
            {
                Show("First coefficient can't be zero", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!isValid)
            {
                Show("Coefficients must be real numbers", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
                
            if (a > maxCoeffValue || b > maxCoeffValue || c > maxCoeffValue|| d > maxCoeffValue)
            {
                Show($"Coefficients can't go out of bounds of an estimated {maxCoeffValue} value", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (a < minCoeffValue || b < minCoeffValue || c < minCoeffValue || d < minCoeffValue)
            {
                Show($"Coefficients can't go out of bounds of an estimated {minCoeffValue} value", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c) || double.IsNaN(d))
            {
                Show("Coefficient cannot be NaN", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsInfinity(a) || double.IsInfinity(b) || double.IsInfinity(c) || double.IsInfinity(d))
            {
                Show("Coefficient cannot reach infinite values", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        
        public static bool TryParse3Coeffs(TextBox t1, TextBox t2, TextBox t3, out double a, out double b, out double c)
        {
            a = 0; b = 0; c = 0;
            double maxCoeffValue = 10000;
            double minCoeffValue = -10000;

            bool isValid = double.TryParse(t1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out a) &&
                           double.TryParse(t2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out b) &&
                           double.TryParse(t3.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out c);
            if (a == 0)
            {
                Show("First coefficient can't be zero", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!isValid)
            {
                Show("Coefficients must be real numbers", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (a > maxCoeffValue || b > maxCoeffValue || c > maxCoeffValue)
            {
                Show($"Coefficients can't go out of bounds of an estimated {maxCoeffValue} value", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (a < minCoeffValue || b < minCoeffValue || c < minCoeffValue)
            {
                Show($"Coefficients can't go out of bounds of an estimated {minCoeffValue} value", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error); 
                return false;
            }
           
            if (double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c))
            {
                Show("Coefficient cannot be NaN", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsInfinity(a) || double.IsInfinity(b) || double.IsInfinity(c))
            {
                Show("Coefficient cannot reach infinite values", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public static bool TryParsePrecision(TextBox precisionBox, out double precision)
        {
            precision = 0;
            const double minAllowedDecimalPlaces = 1;
            const double maxAllowedDecimalPlaces = 14;

            int parsedPrecisionInDp;            
            bool isValid = int.TryParse(precisionBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedPrecisionInDp);

            if (!isValid) 
            {
                Show("Precision must be a valid integer", "Invalid precision", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (parsedPrecisionInDp < minAllowedDecimalPlaces || parsedPrecisionInDp > maxAllowedDecimalPlaces)
            { 
                Show("Precision is out of bounds", "Invalid precision", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            precision = Math.Pow(10, -parsedPrecisionInDp);
            return true;
        }

        public static bool TryParseComplexInitialPoint(TextBox realPart, TextBox imagPart, out Complex complexNumber)
        {
            double real, imag;
            complexNumber = new Complex(0, 0);

            bool isRealPartValid = double.TryParse(
                realPart.Text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out real 
            );

            bool isImaginaryPartValid = double.TryParse(
                imagPart.Text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out imag 
            );

            if (!isRealPartValid || !isImaginaryPartValid || Math.Abs(real) > 10000 || Math.Abs(imag) > 10000)
            {
                Show("Complex number parts are out of bounds", "Invalid initial guess", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!isRealPartValid || !isImaginaryPartValid || Math.Abs(real) < -10000 || Math.Abs(imag) < -10000)
            {
                Show("Complex number parts are out of bounds", "Invalid initial guess", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsNaN(real) || double.IsNaN(imag))
            {
                Show("Initial guess cannot hold NaN values", "Invalid initial guess", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsInfinity(real) || double.IsInfinity(imag))
            {
                Show("Initial guess cannot hold infinite values", "Invalid initial guess", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            complexNumber = new Complex(real, imag);
            return true;
        }

        public static bool TryParseInterval(TextBox leftBound, TextBox rightBound, out double left, out double right)
        {
            left = 0; right = 0;

            bool isLeftValid = double.TryParse(leftBound.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out left);
            bool isRightValid = double.TryParse(rightBound.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out right);

            if (!isLeftValid || !isRightValid || Math.Abs(left) > 10000 || Math.Abs(right) > 10000)
            {
                Show("Interval bounds are out of bounds", "Invalid interval", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!isLeftValid || !isRightValid || Math.Abs(left) < -10000 || Math.Abs(right) < -10000)
            {
                Show("Interval bounds are out of bounds", "Invalid interval", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (left >= right)
            {
                Show("Left bound must be less than right bound", "Invalid interval", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsNaN(left) || double.IsNaN(right))
            {
                Show("Interval cannot hold NaN values", "Invalid interval", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsInfinity(left) || double.IsInfinity(right))
            {
                Show("Interval cannot hold infinite values", "Invalid interval", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        public static int GetDecimalPlaces(double precision)
        {
            return (int)Math.Max(0, Math.Ceiling(-Math.Log10(precision))) + 2; 
        }

     
        public static string FormatComplex(Complex c, int decimalPlaces)
        {
            string realPartString = c.Real.ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture);
            string imagPartString = Math.Abs(c.Imaginary).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture);
            
            double tolerance = Math.Pow(10, -decimalPlaces - 2); 

            if (Math.Abs(c.Imaginary) < tolerance) 
            {
                return realPartString;
            }

            if (Math.Abs(c.Real) < tolerance) 
            {
                return $"{c.Imaginary.ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)}i"; 
            }
           
            return $"{realPartString} {(c.Imaginary >= 0 ? "+" : "-")} {imagPartString}i";
        }
        public static void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string instructions =
                "Welcome to the Complex Nonlinear Equation Solver 2000!\n\n" +
                "**1. Choose Equation Type:**\n" +
                "   - Select Quadratic (az² + bz + c = 0), Cubic (az³ + bz² + cz + d = 0), or Biquadratic (az⁴ + bz² + c = 0).\n" +
                "   - Enter the coefficients in the respective fields. Values should be between -10000 and 10000.\n\n" +
                "**2. Enter Initial Guess (for Newton's method):**\n" +
                "   - If using Newton's method, provide a complex initial guess (real part and imaginary part).\n" +
                "   - This guess helps the algorithm start finding a root.\n\n" +
                "**3. Enter Interval (for Bisection method):**\n" +
                "   - If using the Bisection method, define a real interval [Left Bound, Right Bound].\n" +
                "   - The function must change sign within this interval for the method to work (f(Left) * f(Right) < 0).\n\n" +
                "**4. Enter Precision ε:**\n" +
                "   - Specify the desired precision for the solution. This value should be between 1e-14 and 0.1.\n\n" +
                "**5. Choose the Method:**\n" +
                "   - **Newton:** An iterative method for finding roots, can find complex roots if the initial guess is complex.\n" +
                "   - **Bisection:** A robust method for finding real roots within a given interval. Requires the function to change sign.\n" +
                "   - **Algebraic:** Directly calculates roots using algebraic formulas (available for quadratic, cubic, and biquadratic equations).\n\n" +
                "**6. Solve:**\n" +
                "   - Click the 'Solve' button to calculate the roots based on your inputs.\n" +
                "   - The solution(s) will appear in the 'Solution' box.\n" +
                "   - 'Algorithm complexity' will show the number of iterations (for iterative methods).\n" +
                "   - Errors will be displayed in the 'Error' text area.\n\n" +
                "**7. Graph:**\n" +
                "   - A graph of the real part of the function will be displayed on the right for Bisection method and for Newton's method when the initial guess is primarily real.\n\n" +
                "**8. Save Result:**\n" +
                "   - Click 'Save Result' to save the solution(s) to a 'solution.txt' file.\n\n" +
                "**Note:** For cubic equations, it is highly recommended to use the Algebraic method as iterative methods might struggle with convergence or finding all roots without proper initial guesses.\n" +
                "For complex roots, Newton's method with a complex initial guess is required.\n" +
                "The Bisection method only finds real roots within a given interval.";

            MessageBox.Show(instructions, "How to Use - Instructions", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
