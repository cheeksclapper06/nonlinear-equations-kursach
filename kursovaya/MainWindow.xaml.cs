using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls; 

using kursovaya.Methods;

namespace kursovaya
{
    
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            SolveButton.Click += SolveButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";
            ResultBox.Text = "";
            ComplexityText.Text = "";
            GraphCanvas.Children.Clear();

            
            try
            {
                // Получаем коэффициенты
                double a = double.Parse(CoeffBox1.Text);
                double b = double.Parse(CoeffBox2.Text);
                double c = double.Parse(CoeffBox3.Text);
                double d = double.Parse(CoeffBox4.Text);
                double eps = double.Parse(PrecisionBox.Text);
                
                string method = null!;

                if (NewtonCheck.IsChecked == true)
                {
                    method = "Newton";
                }
                else if (BisectionCheck.IsChecked == true)
                {
                    method = "Bisection";
                }
                else if (AlgebraicCheck.IsChecked == true)
                {
                    method = "Algebraic";
                }


                if (method == null)
                {
                    ErrorText.Text = "Please select a solving method.";
                    return;
                }

                // Вызываем решение
                string result;
                int iterations;

                if (method == "Algebraic")
                {
                    result = SolveAlgebraically(a, b, c, d);
                }
                else
                {
                    result = SolveNumerically(a, b, c, d, eps, method, out iterations);
                    ComplexityText.Text = $"Iterations: {iterations}";
                }

                ResultBox.Text = result;
                DrawGraph(a, b, c, d);
            }
            catch (Exception ex)
            {
                ErrorText.Text = "Error: " + ex.Message;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.WriteAllText("solution.txt", ResultBox.Text);
                MessageBox.Show("Solution saved to solution.txt", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorText.Text = "File save error: " + ex.Message;
            }
        }

        private string SolveAlgebraically(double a, double b, double c, double d)
        {
            if (Math.Abs(a) < 1e-12) return "Not a cubic equation.";

            // Приведение к форме x^3 + px + q = 0
            double p = (3 * a * c - b * b) / (3 * a * a);
            double q = (2 * b * b * b - 9 * a * b * c + 27 * a * a * d) / (27 * a * a * a);
            double offset = -b / (3 * a);
            double discriminant = q * q / 4 + p * p * p / 27;

            string result = "Algebraic roots:\n";

            if (discriminant >= 0)
            {
                // Один вещественный корень, два комплексных
                double sqrtD = Math.Sqrt(discriminant);
                double u = CubeRoot(-q / 2 + sqrtD);
                double v = CubeRoot(-q / 2 - sqrtD);
                double realRoot = u + v + offset;

                result += $"x1 = {realRoot:F6}\n";

                // Комплексные корни
                double real = -0.5 * (u + v) + offset;
                double imag = Math.Sqrt(3) / 2 * (u - v);

                result += $"x2 = {real:F6} + {imag:F6}i\n";
                result += $"x3 = {real:F6} - {imag:F6}i\n";
            }
            else
            {
                // Три вещественных корня
                double phi = Math.Acos(-q / (2 * Math.Sqrt(-p * p * p / 27)));
                double t = 2 * Math.Sqrt(-p / 3);

                double x1 = t * Math.Cos(phi / 3) + offset;
                double x2 = t * Math.Cos((phi + 2 * Math.PI) / 3) + offset;
                double x3 = t * Math.Cos((phi + 4 * Math.PI) / 3) + offset;

                result += $"x1 = {x1:F6}\n";
                result += $"x2 = {x2:F6}\n";
                result += $"x3 = {x3:F6}\n";
            }

            return result;
        }

        private double CubeRoot(double x)
        {
            if (x >= 0)
            {
                return Math.Pow(x, 1.0 / 3.0);
            }
            return -Math.Pow(-x, 1.0 / 3.0);
        }


        private string SolveNumerically(double a, double b, double c, double d, double eps, string method, out int iterations)
        {
            iterations = 0;
            Func<double, double> f = x => a * x * x * x + b * x * x + c * x + d;
            Func<double, double> df = x => 3 * a * x * x + 2 * b * x + c;

            double x0;
            double x1 = 1;
            double root = double.NaN;

            if (method == "Newton")
            {
                x0 = 0;
                do
                {
                    double fx = f(x0);
                    double dfx = df(x0);
                    if (Math.Abs(dfx) < 1e-12) break;
                    x1 = x0 - fx / dfx;
                    iterations++;
                    if (Math.Abs(x1 - x0) < eps) break;
                    x0 = x1;
                } while (iterations < 1000);
                root = x1;
            }
            else if (method == "Bisection")
            {
                x0 = -100;
                x1 = 100;
                while (x1 - x0 > eps && iterations < 1000)
                {
                    double xm = (x0 + x1) / 2;
                    if (f(x0) * f(xm) < 0) x1 = xm;
                    else x0 = xm;
                    iterations++;
                }
                root = (x0 + x1) / 2;
            }

            return $"Approximate root: {root:F6}";
        }

        private void DrawGraph(double a, double b, double c, double d)
{
    GraphCanvas.Children.Clear();

    double width = GraphCanvas.ActualWidth;
    double height = GraphCanvas.ActualHeight;

    if (width == 0 || height == 0)
    {
        return;
    }

    double scaleX = width / 20;
    double scaleY = height / 20;
    double centerX = width / 2;
    double centerY = height / 2;

    // Оси
    var xAxis = new Line { X1 = 0, Y1 = centerY, X2 = width, Y2 = centerY, Stroke = Brushes.Gray, StrokeThickness = 1 };
    var yAxis = new Line { X1 = centerX, Y1 = 0, X2 = centerX, Y2 = height, Stroke = Brushes.Gray, StrokeThickness = 1 };
    GraphCanvas.Children.Add(xAxis);
    GraphCanvas.Children.Add(yAxis);

    // Деления и подписи
    for (int i = -10; i <= 10; i++)
    {
        double xPos = centerX + i * scaleX;
        var tickX = new Line { X1 = xPos, Y1 = centerY - 5, X2 = xPos, Y2 = centerY + 5, Stroke = Brushes.DarkGray };
        GraphCanvas.Children.Add(tickX);

        var labelX = new TextBlock { Text = i.ToString(), Foreground = Brushes.White };
        Canvas.SetLeft(labelX, xPos - 8);
        Canvas.SetTop(labelX, centerY + 6);
        GraphCanvas.Children.Add(labelX);

        double yPos = centerY - i * scaleY;
        var tickY = new Line { X1 = centerX - 5, Y1 = yPos, X2 = centerX + 5, Y2 = yPos, Stroke = Brushes.DarkGray };
        GraphCanvas.Children.Add(tickY);

        if (i != 0)
        {
            var labelY = new TextBlock { Text = i.ToString(), Foreground = Brushes.White };
            Canvas.SetLeft(labelY, centerX + 6);
            Canvas.SetTop(labelY, yPos - 10);
            GraphCanvas.Children.Add(labelY);
        }
    }

    // Анимация графика
    double graphX = -10;
    double prevX = graphX;
    double prevY = a * Math.Pow(prevX, 3) + b * Math.Pow(prevX, 2) + c * prevX + d;

    DispatcherTimer timer = new DispatcherTimer();
    timer.Interval = TimeSpan.FromMilliseconds(5);
    timer.Tick += (s, e) =>
    {
        if (graphX > 10)
        {
            timer.Stop();
            return;
        }

        double y = a * Math.Pow(graphX, 3) + b * Math.Pow(graphX, 2) + c * graphX + d;
        var line = new Line
        {
            X1 = centerX + prevX * scaleX,
            Y1 = centerY - prevY * scaleY,
            X2 = centerX + graphX * scaleX,
            Y2 = centerY - y * scaleY,
            Stroke = Brushes.Lime,
            StrokeThickness = 1.5
        };

        GraphCanvas.Children.Add(line);
        prevX = graphX;
        prevY = y;
        graphX += 0.05;
    };
    timer.Start();
}
    }
}
