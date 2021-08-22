using System;
using System.Windows.Media;
using MathNet.Numerics.Statistics;
using OxyPlot;
using OxyPlot.Wpf;
using WpfApp1.Wpf;
using AreaSeries = OxyPlot.Series.AreaSeries;

namespace WpfApp1
{
    public class RealtimeSeriesViewModel : ViewModel
    {
        private readonly Func<object, double> getValueFromSample;
        private readonly PlotModel model;
        private readonly AreaSeries series;

        private bool isVisible = true;
        private RunningStatistics stats = new();

        public RealtimeSeriesViewModel(string title, string unit, AreaSeries series, PlotModel model,
            Func<object, double> getValueFromSample)
        {
            Title = title;
            Unit = unit;
            this.series = series;
            this.model = model;
            this.getValueFromSample = getValueFromSample;
            Brush = series.Color.ToBrush();
        }

        public string Title { get; }
        public string Unit { get; }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (SetProperty(ref isVisible, value))
                {
                    series.IsVisible = value;
                    model.InvalidatePlot(true);
                }
            }
        }

        public double Max => stats.Maximum;

        public double Min => stats.Minimum;

        public double SampleCount => stats.Count;

        public double Sum { get; private set; }

        public Brush Brush { get; }

        internal void ResetStats()
        {
            stats = new RunningStatistics();
            Sum = 0;
        }

        internal void PushSampleStats(object sample)
        {
            double value = getValueFromSample(sample);
            stats.Push(value);
            Sum += value;

            InvalidateStats();
        }

        private void InvalidateStats()
        {
            OnPropertyChanged(nameof(Max));
            OnPropertyChanged(nameof(Min));
            OnPropertyChanged(nameof(SampleCount));
            OnPropertyChanged(nameof(Sum));
        }
    }
}