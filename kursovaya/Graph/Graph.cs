using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace kursovaya.Graph
{
    public static class Draw
    {
        public static PlotModel CreateFunctionPlotModel(
            Func<double, double> function,
            double zMinPlot = -10,
            double zMaxPlot = 10,
            double step = 0.01,
            string title = "Graph of the f(z) function")
        {
            var model = new PlotModel
            {
                Title = title,
                Background = OxyColors.DarkGray, 
                TitleColor = OxyColors.White  
            };

            var lineSeries = new FunctionSeries(function, zMinPlot, zMaxPlot, step, "f(z)")
            {
                Color = OxyColors.LightGreen 
            };
            model.Series.Add(lineSeries);

            double yMinFunction = lineSeries.MinY;
            double yMaxFunction = lineSeries.MaxY;

            double yIntercept = function(0);
            yMinFunction = Math.Min(yMinFunction, yIntercept);
            yMaxFunction = Math.Max(yMaxFunction, yIntercept);

            yMinFunction = Math.Min(yMinFunction, 0);
            yMaxFunction = Math.Max(yMaxFunction, 0);

            if (Math.Abs(yMaxFunction - yMinFunction) < 1e-9)
            {
                yMinFunction -= 1;
                yMaxFunction += 1;
            }

            double yPadding = (yMaxFunction - yMinFunction) * 0.1;
            yMinFunction -= yPadding;
            yMaxFunction += yPadding;

            var zAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Z",
                Minimum = zMinPlot,
                Maximum = zMaxPlot,
                Key = "YAxis",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = OxyColors.White,      
                TitleColor = OxyColors.White,         
                TextColor = OxyColors.White,          
                TicklineColor = OxyColors.White,      
                MajorGridlineColor = OxyColors.Gray,  
                MinorGridlineColor = OxyColors.DarkGray
            };
            model.Axes.Add(zAxis);

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Y",
                Minimum = yMinFunction,
                Maximum = yMaxFunction,
                Key = "YAxis",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = OxyColors.White,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                MajorGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColors.DarkGray
            };
            model.Axes.Add(yAxis);

            zAxis.MajorStep = (zMaxPlot - zMinPlot) / 10;
            yAxis.MajorStep = (yMaxFunction - yMinFunction) / 10;

            model.InvalidatePlot(true);

            return model;
        }

        public static PlotModel CreateEmptyPlotModel(string message = "Graph is not available")
        {
            var model = new PlotModel
            {
                Title = message,
                Background = OxyColors.Black, 
                TitleColor = OxyColors.White  
            };
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsAxisVisible = false,
                MajorGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColors.DarkGray
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false,
                MajorGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColors.DarkGray
            });
            return model;
        }
    }
}