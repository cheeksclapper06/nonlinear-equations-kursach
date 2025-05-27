using System;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace kursovaya.Graph
{
    public static class Draw
    {
        /// <summary>
        /// Создает и настраивает PlotModel для отображения графика действительной функции.
        /// </summary>
        /// <param name="function">Функция для построения графика (Func<double, double>).</param>
        /// <param name="xMinPlot">Минимальное значение X для построения.</param>
        /// <param name="xMaxPlot">Максимальное значение X для построения.</param>
        /// <param name="step">Шаг для вычисления точек функции.</param>
        /// <param name="title">Заголовок графика.</param>
        /// <returns>Готовая PlotModel для привязки к PlotView.</returns>
        public static PlotModel CreateFunctionPlotModel(
            Func<double, double> function,
            double xMinPlot = -10,
            double xMaxPlot = 10,
            double step = 0.01,
            string title = "График функции f(x)")
        {
            var model = new PlotModel
            {
                Title = title,
                Background = OxyColors.Gray, // Set black background
                TitleColor = OxyColors.White  // White title for visibility
            };

            // 1. Создаем серию данных для функции
            var lineSeries = new FunctionSeries(function, xMinPlot, xMaxPlot, step, "f(x)")
            {
                Color = OxyColors.LightGreen 
            };
            model.Series.Add(lineSeries);

            // 2. Вычисляем y-интервал, включая y = 0
            double yMinFunction = lineSeries.MinY;
            double yMaxFunction = lineSeries.MaxY;

            // Учитываем y-интерцепт (f(0)), если x = 0 находится в диапазоне
            double yIntercept = function(0);
            yMinFunction = Math.Min(yMinFunction, yIntercept);
            yMaxFunction = Math.Max(yMaxFunction, yIntercept);

            // Убедимся, что y = 0 включен в диапазон (чтобы видеть пересечение с x-осью)
            yMinFunction = Math.Min(yMinFunction, 0);
            yMaxFunction = Math.Max(yMaxFunction, 0);

            // Если функция константа или очень плоская
            if (Math.Abs(yMaxFunction - yMinFunction) < 1e-9)
            {
                yMinFunction -= 1;
                yMaxFunction += 1;
            }

            // Добавляем отступ 10%
            double yPadding = (yMaxFunction - yMinFunction) * 0.1;
            yMinFunction -= yPadding;
            yMaxFunction += yPadding;

            // 3. Настраиваем оси X (горизонтальная)
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "X",
                Minimum = xMinPlot,
                Maximum = xMaxPlot,
                Key = "XAxis",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = OxyColors.White,      // White axis line for visibility
                TitleColor = OxyColors.White,         // White axis title
                TextColor = OxyColors.White,          // White tick labels
                TicklineColor = OxyColors.White,      // White tick marks
                MajorGridlineColor = OxyColors.Gray,  // Gray grid lines for contrast
                MinorGridlineColor = OxyColors.DarkGray
            };
            model.Axes.Add(xAxis);

            // 4. Настраиваем оси Y (вертикальная)
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

            // 5. Добавляем сетку на x = 0 и y = 0
            xAxis.MajorStep = (xMaxPlot - xMinPlot) / 10;
            yAxis.MajorStep = (yMaxFunction - yMinFunction) / 10;

            // 6. Обновляем модель
            model.InvalidatePlot(true);

            return model;
        }

        public static PlotModel CreateEmptyPlotModel(string message = "График не доступен для данного метода или функции")
        {
            var model = new PlotModel
            {
                Title = message,
                Background = OxyColors.Black, // Black background for empty plot
                TitleColor = OxyColors.White  // White title for visibility
            };
            // Добавляем оси, чтобы не было пустого пространства
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