using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq; // Add this for Min/Max operations

namespace kursovaya.Graph
{
    public static class GraphP
    {
        public static void Draw(Canvas canvas, Func<double, double> function)
        {
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            if (width <= 0 || height <= 0) return; // Prevent division by zero or negative dimensions

            canvas.Children.Clear();

            // --- Define the X-axis plotting range ---
            double xMinPlot = -10; // Fixed X-axis start
            double xMaxPlot = 10;  // Fixed X-axis end
            double xRange = xMaxPlot - xMinPlot;

            // Calculate initial scaleX based on the desired X-axis plot range
            double scaleX = width / xRange;

            // --- Determine Y-axis dynamic scaling ---
            double yMin = double.MaxValue;
            double yMax = double.MinValue;
            const double step = 0.05; // Use the same step as in the drawing loop

            // First pass: Calculate min and max y values for the given x range
            // Also, consider a slightly larger range to ensure points aren't exactly on the edge
            // and to handle cases where min/max are at the edges of the x range.
            for (double x = xMinPlot; x <= xMaxPlot; x += step)
            {
                try
                {
                    double y = function(x);
                    if (!double.IsNaN(y) && !double.IsInfinity(y)) // Ignore invalid numbers
                    {
                        if (y < yMin) yMin = y;
                        if (y > yMax) yMax = y;
                    }
                }
                catch
                {
                    // Handle potential exceptions during function evaluation (e.g., division by zero)
                    // If an exception occurs, this point is skipped for min/max calculation.
                }
            }

            // Handle cases where yMin/yMax might not have been updated (e.g., function always throws or returns NaN)
            if (yMin == double.MaxValue || yMax == double.MinValue)
            {
                // Fallback to a default reasonable range if no valid Y values found
                yMin = -10;
                yMax = 10;
            }

            // Add padding to the Y-axis range
            double yPadding = (yMax - yMin) * 0.1; // 10% padding
            yMin -= yPadding;
            yMax += yPadding;

            if (Math.Abs(yMax - yMin) < 1e-9) // Prevent division by zero if yMin and yMax are almost identical (e.g., y = constant)
            {
                yMin -= 1; // Expand the range slightly
                yMax += 1;
            }

            double yRange = yMax - yMin;
            double scaleY = height / yRange;

            // Calculate the Y-coordinate of the X-axis (where Y=0 on the graph)
            // This is relative to the canvas's top-left corner (0,0)
            double centerY = height + yMin * scaleY; // Adjust based on dynamic yMin

            // Calculate the X-coordinate of the Y-axis (where X=0 on the graph)
            double centerX = -xMinPlot * scaleX; // Adjust based on fixed xMinPlot

            // --- Draw Axes ---
            // Y-axis (x=0)
            canvas.Children.Add(new Line
            {
                X1 = centerX, Y1 = 0, X2 = centerX, Y2 = height,
                Stroke = Brushes.Gray,
                StrokeThickness = 0.5 // Thinner lines for axes
            });
            // X-axis (y=0)
            canvas.Children.Add(new Line
            {
                X1 = 0, Y1 = centerY, X2 = width, Y2 = centerY,
                Stroke = Brushes.Gray,
                StrokeThickness = 0.5 // Thinner lines for axes
            });

            // --- Draw the function plot ---
            double prevXGraph = centerX + xMinPlot * scaleX;
            double prevYGraph = centerY - function(xMinPlot) * scaleY; // Transform Y value to canvas coordinates

            for (double x = xMinPlot + step; x <= xMaxPlot; x += step)
            {
                double currentY = function(x);

                // Transform real world coordinates to canvas coordinates
                double currentXGraph = centerX + x * scaleX;
                double currentYGraph = centerY - currentY * scaleY; // Y-axis in WPF goes down

                // Only draw if values are finite to prevent drawing lines to infinity
                if (!double.IsNaN(prevYGraph) && !double.IsInfinity(prevYGraph) &&
                    !double.IsNaN(currentYGraph) && !double.IsInfinity(currentYGraph))
                {
                    canvas.Children.Add(new Line
                    {
                        X1 = prevXGraph,
                        Y1 = prevYGraph,
                        X2 = currentXGraph,
                        Y2 = currentYGraph,
                        Stroke = Brushes.Lime,
                        StrokeThickness = 1
                    });
                }
                prevXGraph = currentXGraph;
                prevYGraph = currentYGraph;
            }
        }
    }
}