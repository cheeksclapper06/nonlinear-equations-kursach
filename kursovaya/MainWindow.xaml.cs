using System.Windows;
using System.Numerics;
using OxyPlot;
using OxyPlot.Series;

using kursovaya.Methods.Equations;
using kursovaya.Validations;
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
            HelpButton.Click -= HelpButton_Click;
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

        private void EquationTypeChanged(object? sender, RoutedEventArgs? e)
        {
            QuadraticCoefficients.Visibility = QuadraticCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            CubicCoefficients.Visibility = CubicCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            BiquadraticCoefficients.Visibility = BiquadraticCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MethodTypeChanged(object? sender, RoutedEventArgs? e)
        {
            InitialGuessPanel.Visibility = NewtonCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            BisectionIntervalPanel.Visibility = BisectionCheck.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        public interface IPolynomial
        {
            Complex SolveWithNewton(double eps, Complex z0, out int iters, out TimeSpan elapsed, int maxIters = 1000);
            Complex SolveWithBisection(double left, double right, double eps, out int iters, out TimeSpan elapsed, int maxIters = 1000);
            Complex[] SolveAlgebraically(out TimeSpan elapsed);
        }
        
        private string SolveEquation(IPolynomial equation, string method, double eps, int decimalPlaces, out int iterations, out TimeSpan elapsed, out Complex[]? roots)
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
                    
                    var newtonRoot = equation.SolveWithNewton(eps, guess, out iterations, out elapsed);
                    roots = [newtonRoot];
                    string resultNewton = $"z1: {Validate.FormatComplex(newtonRoot, decimalPlaces)}\n";
                    return resultNewton;

                case "Bisection":
                    if (!Validate.TryParseInterval(LeftBoundBox, RightBoundBox, out double left, out double right))
                    {
                        throw new ArgumentException("Invalid interval bounds.");
                    }
                    var bisectionRoot = equation.SolveWithBisection(left, right, eps, out iterations, out elapsed);
                    roots = [bisectionRoot];
                    string resultBisection = $"z1: {Validate.FormatComplex(bisectionRoot, decimalPlaces)}\n";
                    return resultBisection;

                case "Algebraic":
                    roots = equation.SolveAlgebraically(out elapsed);
                    string resultAlgebraic = "";
                    for (int i = 0; i < roots.Length; i++)
                    {
                        resultAlgebraic += $"z{i + 1}: {Validate.FormatComplex(roots[i], decimalPlaces)}\n";
                    }
                    return resultAlgebraic;

                default:
                    throw new ArgumentException("Invalid method.");
            }
        }
        
        private void SolveButton_Click(object sender, RoutedEventArgs args)
        {
            ResultBox.Text = "";
            ComplexityText.Text = "";

            if (!Validate.TryParsePrecision(PrecisionBox, out double eps))
            {
                Graph.Model = Draw.CreateEmptyPlotModel("Invalid precision");
                return;
            }

            string? selectedMethod = null;
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
            TimeSpan elapsed = TimeSpan.Zero;
            string result = "";
            Func<double, double>? equationFunction = null;
            Complex[]? roots = null;

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
                        result = SolveEquation(quadratic, selectedMethod, eps, decimalPlaces, out iterations, out elapsed, out roots);
                        equationFunction = quadratic.EvaluateReal;
                        break;

                    case "cubic":
                        if (!Validate.TryParse4Coeffs(CoeffBox1Cubic, CoeffBox2Cubic, CoeffBox3Cubic, CoeffBox4Cubic, out double a2, out double b2, out double c2, out double d2))
                        {
                            Graph.Model = Draw.CreateEmptyPlotModel("Invalid coefficients");
                            return;
                        }
                        var cubic = new Cubic(a2, b2, c2, d2);
                        result = SolveEquation(cubic, selectedMethod, eps, decimalPlaces, out iterations, out elapsed, out roots);
                        equationFunction = cubic.EvaluateReal;
                        break;

                    case "biquadratic":
                        if (!Validate.TryParse3Coeffs(CoeffBox1Biquadratic, CoeffBox2Biquadratic, CoeffBox3Biquadratic, out double a3, out double b3, out double c3))
                        {
                            Graph.Model = Draw.CreateEmptyPlotModel("Invalid coefficients");
                            return;
                        }
                        var biquadratic = new Biquadratic(a3, b3, c3);
                        result = SolveEquation(biquadratic, selectedMethod, eps, decimalPlaces, out iterations, out elapsed, out roots);
                        equationFunction = biquadratic.EvaluateReal;
                        break;
                }

                ResultBox.Text = result;

                if (selectedMethod == "Algebraic")
                {
                    ComplexityText.Text = "Algebraic method. Iterations not applicable.";
                    TimeText.Text = $"Time: {elapsed.TotalMilliseconds:F3} ms";
                }
                else
                {
                    ComplexityText.Text = $"Iterations: {iterations}";
                    TimeText.Text = $"Time: {elapsed.TotalMilliseconds:F3} ms";
                }
                
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
                ResultBox.Text = $"Error: {ex.Message}\n";
                Graph.Model = Draw.CreateEmptyPlotModel("Error in plotting function");
            }
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = "solution.txt",
                Title = "Save Solution"
            };

            bool? result = dialogWindow.ShowDialog();
            if (result != true)
            {
                return;
            }
            
            try
            {
                System.IO.File.WriteAllText(dialogWindow.FileName, ResultBox.Text);
                MessageBox.Show($"Solution successfully saved to:\n{dialogWindow.FileName}", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string instructions =
                "Welcome to the Complex Nonlinear Equation Solver 2000!\n\n" +
                "1. Choose Equation Type:\n" +
                "   - Select Quadratic (az² + bz + c = 0), Cubic (az³ + bz² + cz + d = 0), or Biquadratic (az⁴ + bz² + c = 0).\n" +
                "   - Enter the coefficients in the respective fields. Values should be between -10000 and 10000.\n\n" +
                "2. Enter Initial Guess (for Newton's method):\n" +
                "   - If using Newton's method, provide a complex initial guess (real part and imaginary part).\n" +
                "   - This guess helps the algorithm start finding a root.\n\n" +
                "3. Enter Interval (for Bisection method):\n" +
                "   - If using the Bisection method, define a real interval [Left Bound, Right Bound].\n" +
                "   - The function must change sign within this interval for the method to work (f(Left) * f(Right) < 0).\n\n" +
                "4. Enter Precision ε:\n" +
                "   - Specify the desired precision for the solution. This value should be between 1e-14 and 0.1.\n\n" +
                "5. Choose the Method:\n" +
                "   - Newton: An iterative method for finding roots, can find complex roots if the initial guess is complex.\n" +
                "   - Bisection: A robust method for finding real roots within a given interval. Requires the function to change sign.\n" +
                "   - Algebraic Directly calculates roots using algebraic formulas (available for quadratic, cubic, and biquadratic equations).\n\n" +
                "6. Solve:\n" +
                "   - Click the 'Solve' button to calculate the roots based on your inputs.\n" +
                "   - The solution(s) will appear in the 'Solution' box.\n" +
                "   - 'Algorithm complexity' will show the number of iterations (for iterative methods).\n" +
                "   - Any possible error messages will be displayed in a separate window.\n\n" +
                "7. Graph:\n" +
                "   - A graph of the real part of the function will be displayed on the right for Bisection method and for Newton's method when the initial guess is primarily real.\n\n" +
                "8. Save Result:\n" +
                "   - Click 'Save Result' to save the solution(s) to a text file.\n\n" +
                "Note: For cubic equations, it is highly recommended to use the Algebraic method as iterative methods might struggle with convergence or finding all roots without proper initial guesses.\n" +
                "For complex roots, Newton's method with a complex initial guess is required.\n" +
                "The Bisection method only finds real roots within a given interval.";

            MessageBox.Show(instructions, "How to Use - Instructions", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}