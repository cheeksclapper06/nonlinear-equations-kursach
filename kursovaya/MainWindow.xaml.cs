using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using kursovaya.Methods.Equations;
using kursovaya.Validation;
using kursovaya.Graph; // Make sure this is included

namespace kursovaya
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SolveButton.Click += SolveButton_Click;
            SaveButton.Click += SaveButton_Click;

            QuadraticCheck.Checked += EquationTypeChanged;
            CubicCheck.Checked += EquationTypeChanged;
            BiquadraticCheck.Checked += EquationTypeChanged;

            NewtonCheck.Checked += MethodTypeChanged;
            BisectionCheck.Checked += MethodTypeChanged;
            AlgebraicCheck.Checked += MethodTypeChanged;

            EquationTypeChanged(null, null);
            MethodTypeChanged(null, null);
        }

        private void EquationTypeChanged(object sender, RoutedEventArgs e)
        {
            QuadraticCoefficients.Visibility = QuadraticCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            CubicCoefficients.Visibility = CubicCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            BiquadraticCoefficients.Visibility = BiquadraticCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MethodTypeChanged(object sender, RoutedEventArgs e)
        {
            InitialGuessPanel.Visibility = NewtonCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            BisectionIntervalPanel.Visibility = BisectionCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SolveButton_Click(object sender, RoutedEventArgs args)
        {
            ErrorText.Text = "";
            ResultBox.Text = "";
            ComplexityText.Text = "";

            if (!Validation.Validation.TryParseDouble(PrecisionBox, out double eps))
            {
                ErrorText.Text = "Invalid precision.";
                return;
            }

            string selectedMethod = null;
            if (NewtonCheck.IsChecked == true) selectedMethod = "Newton";
            else if (BisectionCheck.IsChecked == true) selectedMethod = "Bisection";
            else if (AlgebraicCheck.IsChecked == true) selectedMethod = "Algebraic";

            if (selectedMethod == null)
            {
                ErrorText.Text = "Please select a method.";
                return;
            }

            int iterations = 0;
            string result = "";
            Func<double, double> equationFunction = null; // This will hold the function to be graphed

            try
            {
                string selectedEquation;
                if (QuadraticCheck.IsChecked == true) selectedEquation = "quadratic";
                else if (CubicCheck.IsChecked == true) selectedEquation = "cubic";
                else if (BiquadraticCheck.IsChecked == true) selectedEquation = "biquadratic";
                else
                {
                    ErrorText.Text = "Please select an equation type.";
                    return;
                }

                int dp = Validation.Validation.GetDecimalPlaces(eps);

                switch (selectedEquation)
                {
                    case "quadratic":
                        if (!Validation.Validation.TryParse3Coeffs(CoeffBox1Quadratic, CoeffBox2Quadratic, CoeffBox3Quadratic, out double a1, out double b1, out double c1))
                        {
                            return;
                        }
                        var quadratic = new Quadratic(a1, b1, c1);
                        result = SolveEquation(quadratic, selectedMethod, eps, dp, out iterations);
                        // Directly assign the EvaluateReal method
                        equationFunction = quadratic.EvaluateReal;
                        break;

                    case "cubic":
                        if (!Validation.Validation.TryParse4Coeffs(CoeffBox1Cubic, CoeffBox2Cubic, CoeffBox3Cubic, CoeffBox4Cubic, out double a2, out double b2, out double c2, out double d2))
                        {
                            return;
                        }
                        var cubic = new Cubic(a2, b2, c2, d2);
                        result = SolveEquation(cubic, selectedMethod, eps, dp, out iterations);
                        // Directly assign the EvaluateReal method
                        equationFunction = cubic.EvaluateReal;
                        break;

                    case "biquadratic":
                        if (!Validation.Validation.TryParse3Coeffs(CoeffBox1Biquadratic, CoeffBox2Biquadratic, CoeffBox3Biquadratic, out double a3, out double b3, out double c3))
                        {
                           return;
                        }
                        var biquadratic = new Biquadratic(a3, b3, c3);
                        result = SolveEquation(biquadratic, selectedMethod, eps, dp, out iterations);
                        equationFunction = biquadratic.EvaluateReal;
                        break;
                }

                ResultBox.Text = result;
                ComplexityText.Text = selectedMethod != "Algebraic" ? $"Iterations: {iterations}" : "Algebraic method. Iterations not applicable.";

                // Draw the graph after the solution is calculated
                if (equationFunction != null)
                {
                    // Make sure you have a Canvas named 'GraphCanvas' in your XAML
                    GraphP.Draw(GraphCanvas, equationFunction);
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error: {ex.Message}";
            }
        }

        private string SolveEquation(dynamic equation, string method, double eps, int decimalPlaces, out int iterations)
        {
            iterations = 0; // Initialize iterations before the switch
            switch (method)
            {
                case "Newton":
                    if (!Validation.Validation.TryParseDouble(InitialGuessBox, out double guess))
                    {
                        throw new ArgumentException("Invalid initial guess.");
                    }
                    return Validation.Validation.FormatComplex(
                        equation.SolveWithNewton(eps, new Complex(guess, 0), out iterations),
                        decimalPlaces);

                case "Bisection":
                    if (!Validation.Validation.TryParseDouble(LeftBoundBox, out double left) ||
                        !Validation.Validation.TryParseDouble(RightBoundBox, out double right))
                    {
                        throw new ArgumentException("Invalid interval bounds.");
                    }
                    return Validation.Validation.FormatComplex(equation.SolveWithBisection(left, right, eps, out iterations), decimalPlaces);

                case "Algebraic":
                    Complex[] roots = equation.SolveAlgebraically();
                    string result = "";
                    for (int i = 0; i < roots.Length; i++)
                    {
                        result += $"Root {i + 1}: {Validation.Validation.FormatComplex(roots[i], decimalPlaces)}\n";
                    }
                    return result;

                default:
                    throw new ArgumentException("Invalid method.");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.WriteAllText("solution.txt", ResultBox.Text);
                MessageBox.Show("Saved to solution.txt", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"File save error: {ex.Message}";
            }
        }
    }
}