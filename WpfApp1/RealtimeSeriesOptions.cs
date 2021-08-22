using System;
using JetBrains.Annotations;

namespace WpfApp1
{
    [PublicAPI]
    public class RealtimeSeriesOptions<T>
    {
        private readonly Func<T, double> valueCallback;

        public RealtimeSeriesOptions(string title, Func<T, double> valueCallback)
        {
            this.valueCallback = valueCallback;
            Title = title;
        }

        public string Title { get; }

        public double GetValueFromSample(object sample)
            => valueCallback((T)sample);
    }
}