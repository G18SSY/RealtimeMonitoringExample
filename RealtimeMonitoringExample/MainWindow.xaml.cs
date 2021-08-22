using System;

namespace RealtimeMonitoringExample
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly TimeSpan[] testVisibleDurations =
        {
            TimeSpan.FromSeconds(30),
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(2),
            TimeSpan.FromMinutes(5)
        };

        private static readonly TimeSpan[] testSamplingIntervals =
        {
            TimeSpan.FromMilliseconds(250),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromSeconds(1)
        };

        public MainWindow()
        {
            InitializeComponent();

            RealtimeSampleOptions<FauxData> options =
                new RealtimeSampleOptions<FauxData>(f => f.Timestamp, FauxData.GetFaux)
                    .WithSeries("Upload", f => f.Upload, "B")
                    .WithSeries("Download", f => f.Download, "B");

            DataContext = new RealtimeMonitorViewModel<FauxData>(options,
                testVisibleDurations,
                testSamplingIntervals);
        }
    }
}