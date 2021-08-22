using System;

namespace RealtimeMonitoringExample
{
    public record FauxData(DateTime Timestamp, double Upload, double Download)
    {
        private static readonly Random random = new();

        public static FauxData GetFaux()
        {
            int upload = random.Next(0, 10000);

            if (upload < 1000)
                upload = 0;

            int download = random.Next(0, 8000);

            FauxData dataItem = new(DateTime.Now, upload, download);

            return dataItem;
        }
    }
}