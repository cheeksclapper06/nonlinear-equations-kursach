using System.Globalization; 
using System.Numerics;
using System.Windows.Controls;
using System.Windows;
using static System.Windows.MessageBox;

namespace kursovaya.Validations
{

    public static class Validate
    {
        public static bool HasTooManyDecimalPlaces(string input, int maxDecimalPlaces = 14)
        {
            if (!decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                return false;
            }

            int[] bits = decimal.GetBits(value);
            int scale = (bits[3] >> 16) & 0xFF;

            return scale > maxDecimalPlaces;
        }
        
        public static bool ContainsComma(params TextBox[] boxes)
        {
            foreach (var box in boxes)
            {
                if (box.Text.Contains(','))
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool TryParse4Coeffs(TextBox t1, TextBox t2, TextBox t3, TextBox t4, out double a, out double b, out double c, out double d)
        {
            a = 0; b = 0; c = 0; d = 0;
            double maxCoeffValue = 10000;
            double minCoeffValue = -10000;

            bool isValid = double.TryParse(t1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out a) &&
                           double.TryParse(t2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out b) &&
                           double.TryParse(t3.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out c) &&
                           double.TryParse(t4.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out d);
            if (!isValid)
            {
                Show("Coefficients must be real numbers", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (HasTooManyDecimalPlaces(t1.Text) || HasTooManyDecimalPlaces(t2.Text) || HasTooManyDecimalPlaces(t3.Text) || HasTooManyDecimalPlaces(t4.Text))
            {
                Show("Coefficients have too many decimal places", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (ContainsComma(t1, t2, t3, t4))
            {
                Show("Coefficients can't contain commas", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (a == 0)
            {
                Show("First coefficient can't be zero", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (a > maxCoeffValue || b > maxCoeffValue || c > maxCoeffValue|| d > maxCoeffValue)
            {
                Show($"Coefficients can't go out of bounds of estimated values", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (a < minCoeffValue || b < minCoeffValue || c < minCoeffValue || d < minCoeffValue)
            {
                Show($"Coefficients can't go out of bounds of estimated values", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c) || double.IsNaN(d))
            {
                MessageBox.Show("Coefficient cannot be NaN", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (!isValid)
            {
                Show("Coefficients must be real numbers", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (HasTooManyDecimalPlaces(t1.Text) || HasTooManyDecimalPlaces(t2.Text) || HasTooManyDecimalPlaces(t3.Text))
            {
                Show("Coefficients have too many decimal places", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (ContainsComma(t1, t2, t3))
            {
                Show("Coefficients can't contain commas", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (a == 0)
            {
                Show("First coefficient can't be zero", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (a > maxCoeffValue || b > maxCoeffValue || c > maxCoeffValue)
            {
                Show("Coefficients can't go out of bounds of estimated values", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (a < minCoeffValue || b < minCoeffValue || c < minCoeffValue)
            {
                Show("Coefficients can't go out of bounds of estimated values", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error); 
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
            
            if (HasTooManyDecimalPlaces(precisionBox.Text))
            {
                Show("Precision has too many decimal places", "Invalid precision", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (parsedPrecisionInDp < minAllowedDecimalPlaces || parsedPrecisionInDp > maxAllowedDecimalPlaces)
            { 
                Show("Precision is out of bounds", "Invalid precision", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (double.IsNaN(parsedPrecisionInDp) )
            {
                Show("Precision cannot be NaN", "Invalid precision", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (double.IsInfinity(parsedPrecisionInDp))
            {
                Show("Coefficient cannot reach infinite values", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (!isRealPartValid || !isImaginaryPartValid)
            {
                Show("Complex number parts must be real numbers", "Invalid initial guess", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (HasTooManyDecimalPlaces(realPart.Text) || HasTooManyDecimalPlaces(imagPart.Text))
            {
                Show("Complex number parts have too many decimal places", "Invalid initial guess", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (ContainsComma(realPart, imagPart))
            {
                Show("Coefficients can't contain commas", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (real > 10000 || imag > 10000)
            {
                Show("Complex number parts are out of bounds", "Invalid initial guess", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (real < -10000 || imag < -10000)
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

            if (!isLeftValid || !isRightValid)
            {
                Show("Interval bounds must be real numbers", "Invalid interval", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (HasTooManyDecimalPlaces(leftBound.Text) || HasTooManyDecimalPlaces(rightBound.Text))
            {
                Show("Interval bounds have too many decimal places", "Invalid interval", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (left > 10000 || right > 10000)
            {
                Show("Interval bounds are out of bounds", "Invalid interval", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if (left < -10000 || right < -10000)
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
           
            return $"z = {realPartString} {(c.Imaginary >= 0 ? "+" : "-")} {imagPartString}i";
        }
    }
}
