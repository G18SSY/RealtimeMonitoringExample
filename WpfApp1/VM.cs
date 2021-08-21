using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace WpfApp1
{
    public class Vm
    {
        private readonly ObservableCollection<FauxData> data = new();

        private readonly DispatcherTimer fauxDataTimer = new()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        private readonly Random random = new();

        private readonly DispatcherTimer scrollerTimer = new()
        {
            Interval = TimeSpan.FromMilliseconds(25)
        };

        private readonly DateTime start = DateTime.Now;

        private readonly Axis xAxis = new LinearAxis
        {
            LabelFormatter = Labeler,
            MajorStep = 5,
            Position = AxisPosition.Bottom,
            TickStyle = TickStyle.None
        };

        private readonly Axis yAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            TickStyle = TickStyle.None,
            MajorStep = 5000,
            Minimum = 0,
            Maximum = 10000
        };

        public Vm()
        {
            Controller.UnbindAll();

            Model.Axes.Add(xAxis);
            Model.Axes.Add(yAxis);
            AreaSeries areaSeries = new()
            {
                ConstantY2 = -1000,
                Fill = OxyColor.FromArgb(100, 52, 183, 235),
                StrokeThickness = 2,
                Color = OxyColor.FromRgb(52, 183, 235),
                Color2 = OxyColors.Transparent,
                Mapping = o =>
                {
                    (DateTime timestamp, double value) = (FauxData)o;
                    double x = (timestamp - start).TotalSeconds;

                    return new DataPoint(x, value);
                },
                ItemsSource = data
            };
            Model.Series.Add(areaSeries);

            fauxDataTimer.Tick += AddMoreData;
            scrollerTimer.Tick += ScrollMore;

            fauxDataTimer.Start();
            scrollerTimer.Start();
        }

        public PlotModel Model { get; } = new();
        public PlotController Controller { get; } = new();

        private static string Labeler(double d)
        {
            if (d < 0)
                return string.Empty;

            return d.ToString("F0") + "s";
        }

        private void ScrollMore(object? sender, EventArgs eventArgs)
        {
            double totalSeconds = (DateTime.Now - start).TotalSeconds;

            xAxis.Minimum = totalSeconds - 60;
            xAxis.Maximum = totalSeconds;

            Model.InvalidatePlot(false);
        }

        private void AddMoreData(object? sender, EventArgs eventArgs)
        {
            int next = random.Next(0, 10000);

            if (next < 1000)
                next = 0;

            FauxData dataItem = new(DateTime.Now, next);
            data.Add(dataItem);

            Model.InvalidatePlot(true);
        }
    }
}