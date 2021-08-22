using System;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace WpfApp1
{
    public class RealtimeMonitorViewModel : ViewModel
    {
        // --------------- To be used in constructor
        private static readonly TimeSpan visibleDuration = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan samplingInterval = TimeSpan.FromSeconds(1);
        private static readonly Func<FauxData> samplingCallback = FauxData.GetFaux;
        // --------------- To be used in constructor

        private readonly TimeBoundCollection<FauxData> data = new((ref FauxData value) => value.Timestamp,
            visibleDuration + TimeSpan.FromSeconds(1));

        private readonly DispatcherTimer samplingTimer = new();
        private readonly DispatcherTimer scrollerTimer = new();
        private readonly DateTime start = DateTime.Now;

        private readonly Axis xAxis = new LinearAxis
        {
            LabelFormatter = Labeler,
            MajorStep = 5,
            Position = AxisPosition.Bottom,
            TickStyle = TickStyle.None,
            TextColor = OxyColors.White,
            AxislineColor = OxyColors.White
        };

        private readonly Axis yAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            TickStyle = TickStyle.None,
            MajorStep = 2500,
            Minimum = 0,
            Maximum = 10000,
            TextColor = OxyColors.White,
            AxislineColor = OxyColors.White
        };

        private bool smoothScroll;

        public RealtimeMonitorViewModel()
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

            samplingTimer.Tick += Sample;
            scrollerTimer.Tick += UpdateScroll;

            samplingTimer.Interval = samplingInterval;
            samplingTimer.Start();

            SetScrollerTimer();
        }

        public bool SmoothScroll
        {
            get => smoothScroll;
            set
            {
                if (SetProperty(ref smoothScroll, value))
                    SetScrollerTimer();
            }
        }

        public PlotModel Model { get; } = new();

        public PlotController Controller { get; } = new();

        private void SetScrollerTimer()
        {
            if (SmoothScroll)
            {
                scrollerTimer.Interval = TimeSpan.FromMilliseconds(35);
                scrollerTimer.Start();
            }
            else
            {
                scrollerTimer.Stop();
            }
        }

        private static string Labeler(double d)
        {
            if (d < 0)
                return string.Empty;

            return d.ToString("F0") + "s";
        }

        private void UpdateScroll(object? sender, EventArgs eventArgs)
            => UpdateScroll(true);

        private void UpdateScroll(bool invalidatePlot)
        {
            TimeSpan elapsed = DateTime.Now - start;

            xAxis.Minimum = (elapsed - visibleDuration).TotalSeconds;
            xAxis.Maximum = elapsed.TotalSeconds;

            if (invalidatePlot)
                Model.InvalidatePlot(false);
        }

        private void Sample(object? sender, EventArgs eventArgs)
            => Sample();

        private void Sample()
        {
            data.Add(samplingCallback());

            if (!SmoothScroll)
                UpdateScroll(false);

            Model.InvalidatePlot(true);
        }
    }
}