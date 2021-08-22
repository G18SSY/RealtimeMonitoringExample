using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RealtimeMonitoringExample.Wpf
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }

        public Visibility FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool bValue)
                throw new ArgumentException("Value is not a bool", nameof(value));

            return bValue ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;

            if (Equals(value, FalseValue))
                return false;

            throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}