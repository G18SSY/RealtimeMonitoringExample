using System;
using JetBrains.Annotations;

namespace RealtimeMonitoringExample
{
    [PublicAPI]
    public class RealtimeSeriesOptions<T>
    {
        private readonly Func<T, double> valueCallback;

        public RealtimeSeriesOptions(string title, string? unit, Func<T, double> valueCallback)
        {
            this.valueCallback = valueCallback;
            Title = title;
            Unit = unit ?? string.Empty;
        }

        public string Title { get; }
        
        public string Unit { get; }

        public double GetValueFromSample(object sample)
            => valueCallback((T)sample);
    }
}