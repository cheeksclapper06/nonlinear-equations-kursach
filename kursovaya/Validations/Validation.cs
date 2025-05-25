using System;
using System.Globalization; // Для CultureInfo.InvariantCulture, NumberStyles
using System.Numerics;
using System.Runtime.InteropServices; // Для Complex
using System.Windows.Controls;
using System.Windows;
using static System.Windows.MessageBox; // Для TextBox

namespace kursovaya.Validation
{

    public static class Validation
    {
        public static bool TryParse4Coeffs(TextBox t1, TextBox t2, TextBox t3, TextBox t4, out double a, out double b, out double c, out double d)
        {
            a = 0; b = 0; c = 0; d = 0;
            bool isValid = double.TryParse(t1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out a) &&
                           double.TryParse(t2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out b) &&
                           double.TryParse(t3.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out c) &&
                           double.TryParse(t4.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out d);
            if (!isValid || Math.Abs(a) >= 1e308 || Math.Abs(b) >= 1e308 || Math.Abs(c) >= 1e308 || Math.Abs(d) >= 1e308)
            {
                Show("Coefficient cannot reach infinity", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!isValid || Math.Abs(a) <= -1e-308 || Math.Abs(b) <= 1e-308 || Math.Abs(c) <= 1e-308 || Math.Abs(d) <= 1e-308)
            {
                Show("Coefficient cannot reach infinity", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        
        public static bool TryParse3Coeffs(TextBox t1, TextBox t2, TextBox t3, out double a, out double b, out double c)
        {
            a = 0; b = 0; c = 0;
            bool isValid = double.TryParse(t1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out a) &&
                           double.TryParse(t2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out b) &&
                           double.TryParse(t3.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out c);
            if (!isValid || Math.Abs(a) >= 1e308 || Math.Abs(b) >= 1e308 || Math.Abs(c) >= 1e308)
            {
                Show("Coefficient cannot reach infinity", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!isValid || Math.Abs(a) <= -1e-308 || Math.Abs(b) <= 1e-308 || Math.Abs(c) <= 1e-308)
            {
                Show("Coefficient cannot reach infinity", "Invalid coefficients", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
       
        public static bool TryParseDouble(TextBox box, out double value)
        {
            if (!double.TryParse(box.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                Show("Неверный формат ввода для точности. Пожалуйста, введите число.", "Precision input error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            const double MinAllowedPrecision = 1e-14;
            const double MaxAllowedPrecision = 1e-1;

            if (value < MinAllowedPrecision)
            {
                Show($"Требуемая точность слишком высока для типа данных double. Пожалуйста, введите значение не менее {MinAllowedPrecision:E0}.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (value > MaxAllowedPrecision)
            {
                Show($"Требуемая точность слишком низка. Пожалуйста, введите значение не более {MaxAllowedPrecision:E0}.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Также убедитесь, что точность не является Infinity или NaN
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                Show("Точность должна быть конечным числом.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // --- Конец новых ограничений ---

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
    }
}
