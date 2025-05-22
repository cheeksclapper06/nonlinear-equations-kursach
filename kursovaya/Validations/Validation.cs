using System;
using System.Globalization; // Для CultureInfo.InvariantCulture, NumberStyles
using System.Numerics; // Для Complex
using System.Windows.Controls; // Для TextBox

namespace kursovaya.Validation
{

    public static class Validation
    {
       
        public static bool TryParseCoeffs(TextBox t1, TextBox t2, TextBox t3, out double a, out double b, out double c)
        {
            a = 0; b = 0; c = 0;
            // Используем InvariantCulture для надежного парсинга чисел (с точкой в качестве десятичного разделителя)
            return double.TryParse(t1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out a) &&
                   double.TryParse(t2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out b) &&
                   double.TryParse(t3.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out c);
        }

       
        public static bool TryParseCoeffs(TextBox t1, TextBox t2, TextBox t3, TextBox t4,
            out double a, out double b, out double c, out double d)
        {
            a = 0; b = 0; c = 0; d = 0;
            return double.TryParse(t1.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out a) &&
                   double.TryParse(t2.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out b) &&
                   double.TryParse(t3.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out c) &&
                   double.TryParse(t4.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out d);
        }
       
        public static bool TryParseDouble(TextBox box, out double value)
        {
            return double.TryParse(box.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
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
