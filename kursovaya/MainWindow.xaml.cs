using System.Windows;
using System.Numerics;

using OxyPlot;
using OxyPlot.Series;

using kursovaya.Methods.Equations;
using kursovaya.Validation;
using kursovaya.Graph;


namespace kursovaya
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SolveButton.Click += SolveButton_Click;
            SaveButton.Click += SaveButton_Click;
            HelpButton.Click += HelpButton_Click;

            QuadraticCheck.Checked += EquationTypeChanged;
            QuadraticCheck.Unchecked += EquationTypeChanged;
            CubicCheck.Checked += EquationTypeChanged;
            CubicCheck.Unchecked += EquationTypeChanged;
            BiquadraticCheck.Checked += EquationTypeChanged;
            BiquadraticCheck.Unchecked += EquationTypeChanged;

            NewtonCheck.Checked += MethodTypeChanged;
            NewtonCheck.Unchecked += MethodTypeChanged;
            BisectionCheck.Checked += MethodTypeChanged;
            BisectionCheck.Unchecked += MethodTypeChanged;
            AlgebraicCheck.Checked += MethodTypeChanged;
            AlgebraicCheck.Unchecked += MethodTypeChanged;

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

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Validate.HelpButton_Click(sender, e);
        }

        private void SolveButton_Click(object sender, RoutedEventArgs args)
        {
            ErrorText.Text = "";
            ResultBox.Text = "";
            ComplexityText.Text = "";

            // Validate precision
            if (!Validate.TryParsePrecision(PrecisionBox, out double eps))
            {
                Graph.Model = Draw.CreateEmptyPlotModel("Invalid precision");
                return;
            }

            string selectedMethod = null;
            if (NewtonCheck.IsChecked == true) selectedMethod = "Newton";
            else if (BisectionCheck.IsChecked == true) selectedMethod = "Bisection";
            else if (AlgebraicCheck.IsChecked == true) selectedMethod = "Algebraic";

            if (selectedMethod == null)
            {
                Graph.Model = Draw.CreateEmptyPlotModel("No method selected");
                return;
            }

            int decimalPlaces = int.Parse(PrecisionBox.Text);
            if (decimalPlaces < 1 || decimalPlaces > 14)
            {
                Graph.Model = Draw.CreateEmptyPlotModel("Invalid precision");
                return;
            }

            int iterations = 0;
            string result = "";
            Func<double, double> equationFunction = null;
            Complex[] roots = null;

            try
            {
                string selectedEquation;
                if (QuadraticCheck.IsChecked == true) selectedEquation = "quadratic";
                else if (CubicCheck.IsChecked == true) selectedEquation = "cubic";
                else if (BiquadraticCheck.IsChecked == true) selectedEquation = "biquadratic";
                else
                {
                    Graph.Model = Draw.CreateEmptyPlotModel("No equation type selected");
                    return;
                }

                switch (selectedEquation)
                {
                    case "quadratic":
                        if (!Validate.TryParse3Coeffs(CoeffBox1Quadratic, CoeffBox2Quadratic, CoeffBox3Quadratic, out double a1, out double b1, out double c1))
                        {
                            Graph.Model = Draw.CreateEmptyPlotModel("Invalid coefficients");
                            return;
                        }
                        var quadratic = new Quadratic(a1, b1, c1);
                        result = SolveEquation(quadratic, selectedMethod, eps, decimalPlaces, out iterations, out roots);
                        equationFunction = quadratic.EvaluateReal;
                        break;

                    case "cubic":
                        if (!Validate.TryParse4Coeffs(CoeffBox1Cubic, CoeffBox2Cubic, CoeffBox3Cubic, CoeffBox4Cubic, out double a2, out double b2, out double c2, out double d2))
                        {
                            Graph.Model = Draw.CreateEmptyPlotModel("Invalid coefficients");
                            return;
                        }
                        var cubic = new Cubic(a2, b2, c2, d2);
                        result = SolveEquation(cubic, selectedMethod, eps, decimalPlaces, out iterations, out roots);
                        equationFunction = cubic.EvaluateReal;
                        break;

                    case "biquadratic":
                        if (!Validate.TryParse3Coeffs(CoeffBox1Biquadratic, CoeffBox2Biquadratic, CoeffBox3Biquadratic, out double a3, out double b3, out double c3))
                        {
                            Graph.Model = Draw.CreateEmptyPlotModel("Invalid coefficients");
                            return;
                        }
                        var biquadratic = new Biquadratic(a3, b3, c3);
                        result = SolveEquation(biquadratic, selectedMethod, eps, decimalPlaces, out iterations, out roots);
                        equationFunction = biquadratic.EvaluateReal;
                        break;
                }

                ResultBox.Text = result;
                ComplexityText.Text = selectedMethod != "Algebraic" ? $"Iterations: {iterations}" : "Algebraic method. Iterations not applicable.";

                if (equationFunction != null)
                {
                    double zMin = -10;
                    double zMax = 10;
                    if (roots != null && roots.Any(r => Math.Abs(r.Imaginary) < 1e-10))
                    {
                        var realRoots = roots.Where(r => Math.Abs(r.Imaginary) < 1e-10).Select(r => r.Real).ToList();
                        zMin = Math.Min(zMin, realRoots.Min() - 2);
                        zMax = Math.Max(zMax, realRoots.Max() + 2);
                    }

                    var plotModel = Draw.CreateFunctionPlotModel(
                        equationFunction,
                        zMinPlot: zMin,
                        zMaxPlot: zMax,
                        step: 0.01,
                        title: $"{selectedEquation} Equation Graph"
                    );

                    if (roots != null)
                    {
                        var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 5, MarkerFill = OxyColors.Red };
                        foreach (var root in roots)
                        {
                            if (Math.Abs(root.Imaginary) < 1e-10) 
                            {
                                scatterSeries.Points.Add(new ScatterPoint(root.Real, equationFunction(root.Real)));
                            }
                        }
                        plotModel.Series.Add(scatterSeries);
                    }
                    Graph.Model = plotModel;
                }
                else
                {
                    Graph.Model = Draw.CreateEmptyPlotModel("No function to plot");
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error: {ex.Message}";
                Graph.Model = Draw.CreateEmptyPlotModel("Error in plotting function");
            }
        }

        private string SolveEquation(dynamic equation, string method, double eps, int decimalPlaces, out int iterations, out Complex[] roots)
        {
            iterations = 0;
            roots = null;
            switch (method)
            {
                case "Newton":
                    if (!Validate.TryParseComplexInitialPoint(InitialGuessBoxReal, InitialGuessBoxImaginary, out Complex guess))
                    {
                        throw new ArgumentException("Invalid initial guess.");
                    }
                    var newtonRoot = equation.SolveWithNewton(eps, guess, out iterations);
                    roots = new Complex[] { newtonRoot };
                    return Validate.FormatComplex(newtonRoot, decimalPlaces);

                case "Bisection":
                    if (!Validate.TryParseInterval(LeftBoundBox, RightBoundBox, out double left, out double right))
                    {
                        throw new ArgumentException("Invalid interval bounds.");
                    }
                    var bisectionRoot = equation.SolveWithBisection(left, right, eps, out iterations);
                    roots = new Complex[] { bisectionRoot };
                    return Validate.FormatComplex(bisectionRoot, decimalPlaces);

                case "Algebraic":
                    roots = equation.SolveAlgebraically();
                    string result = "";
                    for (int i = 0; i < roots.Length; i++)
                    {
                        result += $"z{i + 1}: {Validate.FormatComplex(roots[i], decimalPlaces)}\n";
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
                System.IO.File.WriteAllText("solution.txt", ResultBox.Text);
                MessageBox.Show("Saved to solution.txt", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"File save error: {ex.Message}";
                Graph.Model = Draw.CreateEmptyPlotModel("Error saving file");
            }
        }
    }
}