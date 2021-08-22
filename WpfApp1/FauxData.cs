using System;

namespace WpfApp1
{
    public record FauxData(DateTime Timestamp, double Value)
    {
        private static readonly Random random = new();

        public static FauxData GetFaux()
        {
            int next = random.Next(0, 10000);

            if (next < 1000)
                next = 0;

            FauxData dataItem = new(DateTime.Now, next);

            return dataItem;
        }
    }
}