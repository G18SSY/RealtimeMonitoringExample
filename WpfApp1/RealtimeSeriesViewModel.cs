using System.Windows.Media;
using OxyPlot;
using OxyPlot.Wpf;
using AreaSeries = OxyPlot.Series.AreaSeries;

namespace WpfApp1
{
    public class RealtimeSeriesViewModel : ViewModel
    {
        private readonly PlotModel model;
        private readonly AreaSeries series;

        private bool isVisible = true;

        public RealtimeSeriesViewModel(string title, AreaSeries series, PlotModel model)
        {
            Title = title;
            this.series = series;
            this.model = model;
            Brush = series.Color.ToBrush();
        }

        public string Title { get; }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (SetProperty(ref isVisible, value))
                {
                    series.IsVisible = value;
                    model.InvalidatePlot(false);
                }
            }
        }

        public Brush Brush { get; }
    }
}