using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using WpfApp1.Wpf;

namespace WpfApp1
{
    public class RealtimeMonitorViewModel<T> : ViewModel, IRealtimeMonitorViewModel
    {
        private readonly Queue<OxyColor> availableColors = new(GenerateColorOrder());

        private static IEnumerable<OxyColor> GenerateColorOrder()
        {
            yield return OxyColor.Parse("#4575b4");
            yield return OxyColor.Parse("#91bfdb");
            yield return OxyColor.Parse("#e0f3f8");
        }

        private readonly TimeBoundCollection<T> data;
        private readonly RealtimeSampleOptions<T> options;
        private readonly DispatcherTimer samplingTimer = new();
        private readonly DispatcherTimer scrollerTimer = new();
        private DateTime start = DateTime.Now;

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
            MajorStep = 2500,
            Minimum = 0,
            Maximum = 10000,
            MajorGridlineStyle = LineStyle.Solid
        };

        private bool isEnabled = true;
        private TimeSpan samplingInterval;
        private bool smoothScroll;
        private TimeSpan visibleDuration;

        internal RealtimeMonitorViewModel(RealtimeSampleOptions<T> options, IEnumerable<TimeSpan> visibleDurations,
            IEnumerable<TimeSpan> samplingIntervals)
        {
            this.options = options;
            PossibleVisibleDurations = visibleDurations.ToList();
            PossibleSamplingIntervals = samplingIntervals.ToList();

            VisibleDuration = PossibleVisibleDurations[0];
            SamplingInterval = PossibleSamplingIntervals[0];

            data = new TimeBoundCollection<T>(options.GetTimestamp,
                PossibleVisibleDurations.Max() + PossibleSamplingIntervals.Max());

            InitializeController();
            Series = InitializeModel().ToList();

            samplingTimer.Tick += Sample;
            scrollerTimer.Tick += UpdateScroll;

            RestartCommand = new Command(Reset);
            StopCommand = new Command(Stop);
            
            RefreshScrollerTimer();
            RefreshSamplingTimer();
        }

        private void Stop()
        {
            IsEnabled = false;
        }

        private void Reset()
        {
            data.Clear();
            start = DateTime.Now;
            IsEnabled = true;
            
            UpdateScroll(true);
            
            RefreshSamplingTimer();
            RefreshScrollerTimer();
        }

        private void RefreshSamplingTimer()
        {
            samplingTimer.Interval = SamplingInterval;
            
            if (IsEnabled)
            {
                samplingTimer.Start();
            }
            else
            {
                samplingTimer.Stop();
            }
        }

        public bool SmoothScroll
        {
            get => smoothScroll;
            set
            {
                if (SetProperty(ref smoothScroll, value))
                    RefreshScrollerTimer();
            }
        }

        public IReadOnlyList<TimeSpan> PossibleVisibleDurations { get; }

        public TimeSpan VisibleDuration
        {
            get => visibleDuration;
            set
            {
                if (SetProperty(ref visibleDuration, GetClosestFromList(PossibleVisibleDurations, value)))
                {
                    UpdateScroll(true);
                }
            }
        }

        public IReadOnlyList<TimeSpan> PossibleSamplingIntervals { get; }

        public TimeSpan SamplingInterval
        {
            get => samplingInterval;
            set
            {
                if (SetProperty(ref samplingInterval, GetClosestFromList(PossibleSamplingIntervals, value)))
                {
                    RefreshSamplingTimer();
                }
            }
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (SetProperty(ref isEnabled, value))
                {
                    RefreshSamplingTimer();
                    RefreshScrollerTimer();
                }
            }
        }

        public PlotModel Model { get; } = new();

        public PlotController Controller { get; } = new();
        
        public ICommand RestartCommand { get; }
        
        public ICommand StopCommand { get; }

        public IReadOnlyList<RealtimeSeriesViewModel> Series { get; }

        private void InitializeController()
            => Controller.UnbindAll();

        private IEnumerable<RealtimeSeriesViewModel> InitializeModel()
        {
            Model.Axes.Add(xAxis);
            Model.Axes.Add(yAxis);

            foreach (RealtimeSeriesOptions<T> seriesOptions in options.Series)
            {
                AreaSeries series = CreateSampleSeries(seriesOptions);
                Model.Series.Add(series);

                yield return new RealtimeSeriesViewModel(seriesOptions.Title, series, Model);
            }
        }

        private AreaSeries CreateSampleSeries(RealtimeSeriesOptions<T> seriesOptions)
        {
            OxyColor color = availableColors.Dequeue();

            AreaSeries areaSeries = new()
            {
                ConstantY2 = -1000,
                Fill = OxyColor.FromArgb(30, color.R, color.G, color.B),
                StrokeThickness = 2,
                Color = color,
                Color2 = OxyColors.Transparent,
                Mapping = MapSampleToPoint,
                ItemsSource = data
            };

            return areaSeries;

            DataPoint MapSampleToPoint(object sample)
            {
                return GetDataPointFromSample(sample, seriesOptions);
            }
        }

        private static TimeSpan GetClosestFromList(IEnumerable<TimeSpan> options, TimeSpan value)
        {
            // Use MinBy instead
            return options.OrderBy(o => Math.Abs(o.TotalMilliseconds - value.TotalMilliseconds)).First();
        }

        private DataPoint GetDataPointFromSample(object sample, RealtimeSeriesOptions<T> seriesOptions)
        {
            DateTime timestamp = options.GetTimestamp(sample);
            double x = (timestamp - start).TotalSeconds;
            double y = seriesOptions.GetValueFromSample(sample);

            return new DataPoint(x, y);
        }

        private void RefreshScrollerTimer()
        {
            scrollerTimer.Interval = TimeSpan.FromMilliseconds(20);

            if (SmoothScroll && IsEnabled)
            {
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

            xAxis.Minimum = (elapsed - VisibleDuration).TotalSeconds;
            xAxis.Maximum = elapsed.TotalSeconds;

            if (invalidatePlot)
                Model.InvalidatePlot(false);
        }

        private void Sample(object? sender, EventArgs eventArgs)
            => Sample();

        private void Sample()
        {
            T sample = options.TakeSample();
            data.Add(sample);

            if (!SmoothScroll)
                UpdateScroll(false);

            Model.InvalidatePlot(true);
        }
    }
}