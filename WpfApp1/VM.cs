using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Threading;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WpfApp1
{
    public class Vm
    {
        private readonly ObservableCollection<ObservablePoint> data = new();

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

        private readonly Axis xAxis = new()
        {
            Labeler = Labeler,
            MinStep = 5,
            ForceStepToMin = true
        };

        private static string Labeler(double d)
        {
            if(d<0)
                return string.Empty;
            
            return d.ToString("F0") + "s";
        }

        public Vm()
        {
           XAxes.Add(xAxis);

            Series.Add(new LineSeries<ObservablePoint>
            {
                GeometrySize = 0,
                Fill = new SolidColorPaint(SKColor.Parse("#d853fc").WithAlpha(50)),
                LineSmoothness = 0,
                Stroke = new SolidColorPaint(SKColors.Blue)
                {
                    StrokeThickness = 2
                },
                Values = data
            });

            fauxDataTimer.Tick += AddMoreData;
            scrollerTimer.Tick += ScrollMore;

            fauxDataTimer.Start();
            scrollerTimer.Start();
        }

        public ObservableCollection<ISeries> Series { get; } = new();
        public ObservableCollection<IAxis> XAxes { get; } = new();

        private void ScrollMore(object? sender, EventArgs eventArgs)
        {
            double totalSeconds = (DateTime.Now - start).TotalSeconds;

            xAxis.MinLimit = totalSeconds - 60;
            xAxis.MaxLimit = totalSeconds;
        }

        private void AddMoreData(object? sender, EventArgs eventArgs)
        {
            int next = random.Next(0, 10000);

            if (next < 1000)
                next = 0;

            double x = (DateTime.Now - start).TotalSeconds;
            data.Add(new ObservablePoint(x, next));
        }
    }

    public record FauxData(DateTime Timestamp, double Value);
}