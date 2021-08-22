using System;
using System.Collections.Generic;

namespace RealtimeMonitoringExample
{
    public class RealtimeSampleOptions<T>
    {
        private readonly Func<T> getSampleCallback;
        private readonly List<RealtimeSeriesOptions<T>> series = new();
        private readonly Func<T, DateTime> timestampCallback;

        public RealtimeSampleOptions(Func<T, DateTime> timestampCallback, Func<T> getSampleCallback)
        {
            this.timestampCallback = timestampCallback;
            this.getSampleCallback = getSampleCallback;
        }

        public IReadOnlyList<RealtimeSeriesOptions<T>> Series => series;

        public RealtimeSampleOptions<T> WithSeries(string title, Func<T, double> valueCallback, string? unit = null)
        {
            series.Add(new RealtimeSeriesOptions<T>(title, unit, valueCallback));

            return this;
        }

        public DateTime GetTimestamp(T sample)
            => timestampCallback(sample);
        
        public DateTime GetTimestamp(object sample)
            => timestampCallback((T)sample);

        public T TakeSample()
            => getSampleCallback();
    }
}