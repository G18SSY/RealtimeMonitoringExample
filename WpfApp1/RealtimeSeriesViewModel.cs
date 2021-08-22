using OxyPlot.Series;

namespace WpfApp1
{
    public class RealtimeSeriesViewModel : ViewModel
    {
        private readonly AreaSeries series;
        private bool isVisible = true;

        public RealtimeSeriesViewModel(string title, AreaSeries series)
        {
            Title = title;
            this.series = series;
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
                    series.PlotModel?.InvalidatePlot(false);
                }
            }
        }
    }
}