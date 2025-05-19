namespace kursovaya.Methods;

public class Methods
{
     private string Algebraical_Method(double a, double b, double c, double d)
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
}