using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using kursovaya.Methods.Equations;
using kursovaya.Validation;

namespace kursovaya
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SolveButton.Click += SolveButton_Click;
            SaveButton.Click += SaveButton_Click;
            PlotInDesmosButton.Click += PlotInDesmosButton_Click;

            QuadraticCheck.Checked += EquationTypeChanged;
            CubicCheck.Checked += EquationTypeChanged;
            BiquadraticCheck.Checked += EquationTypeChanged;

            NewtonCheck.Checked += MethodTypeChanged;
            BisectionCheck.Checked += MethodTypeChanged;
            AlgebraicCheck.Checked += MethodTypeChanged;

            EquationTypeChanged(null, null);
            MethodTypeChanged(null, null);
            InitializeWebView2();
        }

        private void InitializeWebView2()
        {
            try
            {
                DesmosWebView.EnsureCoreWebView2Async(null).GetAwaiter().GetResult();
                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "desmos_graph.html");
                if (File.Exists(htmlPath))
                {
                    DesmosWebView.CoreWebView2.Navigate(htmlPath);
                }
                else
                {
                    ErrorText.Text = "Desmos HTML file not found.";
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"WebView2 initialization failed: {ex.Message}";
            }
        }

        private void EquationTypeChanged(object sender, RoutedEventArgs e)
        {
            QuadraticCoefficients.Visibility = QuadraticCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            CubicCoefficients.Visibility = CubicCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            BiquadraticCoefficients.Visibility = BiquadraticCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MethodTypeChanged(object sender, RoutedEventArgs e)
        {
            if (NewtonCheck.IsChecked == true)
            {
                InitialGuessPanel.Visibility = Visibility.Visible;
                BisectionIntervalPanel.Visibility = Visibility.Collapsed;
            }
            else if (BisectionCheck.IsChecked == true)
            {
                InitialGuessPanel.Visibility = Visibility.Collapsed;
                BisectionIntervalPanel.Visibility = Visibility.Visible;
            }
            else
            {
                InitialGuessPanel.Visibility = Visibility.Collapsed;
                BisectionIntervalPanel.Visibility = Visibility.Collapsed;
            }
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

            string? selectedMethod = null;
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
                string latexEquation = "";

                switch (selectedEquation)
                {
                    case "quadratic":
                        if (!Validation.Validation.TryParseCoeffs(CoeffBox1Quadratic, CoeffBox2Quadratic, CoeffBox3Quadratic, out double a1, out double b1, out double c1))
                        {
                            ErrorText.Text = "Invalid quadratic coefficients.";
                            return;
                        }
                        var quadratic = new Quadratic(a1, b1, c1);
                        result = SolveEquation(quadratic, selectedMethod, eps, dp, out iterations);
                        latexEquation = BuildLatex(a1, "x^2") + BuildLatex(b1, "x") + BuildLatex(c1, "", true);
                        break;

                    case "cubic":
                        if (!Validation.Validation.TryParseCoeffs(CoeffBox1Cubic, CoeffBox2Cubic, CoeffBox3Cubic, CoeffBox4Cubic, out double a2, out double b2, out double c2, out double d2))
                        {
                            ErrorText.Text = "Invalid cubic coefficients.";
                            return;
                        }
                        var cubic = new Cubic(a2, b2, c2, d2);
                        result = SolveEquation(cubic, selectedMethod, eps, dp, out iterations);
                        latexEquation = BuildLatex(a2, "x^3") + BuildLatex(b2, "x^2") + BuildLatex(c2, "x") + BuildLatex(d2, "", true);
                        break;

                    case "biquadratic":
                        if (!Validation.Validation.TryParseCoeffs(CoeffBox1Biquadratic, CoeffBox2Biquadratic, CoeffBox3Biquadratic, out double a3, out double b3, out double c3))
                        {
                            ErrorText.Text = "Invalid biquadratic coefficients.";
                            return;
                        }
                        var biquadratic = new Biquadratic(a3, b3, c3);
                        result = SolveEquation(biquadratic, selectedMethod, eps, dp, out iterations);
                        latexEquation = BuildLatex(a3, "x^4") + BuildLatex(b3, "x^2") + BuildLatex(c3, "", true);
                        break;
                }

                ResultBox.Text = result;
                ComplexityText.Text = selectedMethod != "Algebraic" ? $"Iterations: {iterations}" : "Algebraic method. Iterations not applicable.";

                SendEquationToDesmos(latexEquation);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error: {ex.Message}";
            }
        }

        private string SolveEquation(dynamic equation, string method, double eps, int decimalPlaces, out int iterations)
        {
            iterations = 0;
            switch (method)
            {
                case "Newton":
                    var guess = double.Parse(InitialGuessBox.Text, CultureInfo.InvariantCulture);
                    return Validation.Validation.FormatComplex(
                        equation.SolveWithNewton(eps, new Complex(guess, 0), out iterations),
                        decimalPlaces);

                case "Bisection":
                    var left = double.Parse(LeftBoundBox.Text, CultureInfo.InvariantCulture);
                    var right = double.Parse(RightBoundBox.Text, CultureInfo.InvariantCulture);
                    return Validation.Validation.FormatComplex(
                        equation.SolveWithBisection(left, right, eps, out iterations),
                        decimalPlaces);

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

        private void SendEquationToDesmos(string latexEquation)
        {
            if (DesmosWebView.CoreWebView2 == null)
            {
                ErrorText.Text = "Desmos is not ready.";
                return;
            }

            if (latexEquation.StartsWith("+")) latexEquation = latexEquation.Substring(1);
            latexEquation = latexEquation.Replace("+-", "-");

            string escapedLatex = latexEquation.Replace("\\", "\\\\").Replace("\"", "\\\"");
            string json = $"{{\"type\":\"plotEquation\",\"equation\":\"{escapedLatex}\"}}";

            try
            {
                DesmosWebView.CoreWebView2.PostWebMessageAsJson(json);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Desmos plot error: {ex.Message}";
            }
        }

        private string BuildLatex(double coeff, string variable, bool isLast = false)
        {
            if (coeff == 0) return isLast ? "0" : "";
            string sign = coeff > 0 ? "+" : "-";
            string absVal = Math.Abs(coeff) == 1 && variable != "" ? "" : Math.Abs(coeff).ToString(CultureInfo.InvariantCulture);
            return sign + absVal + variable;
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

        private void PlotInDesmosButton_Click(object sender, RoutedEventArgs e)
        {
            SolveButton_Click(sender, e);
        }
    }
}
