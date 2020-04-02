using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SDLP.Converters
{
    class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (!string.IsNullOrEmpty((string)value))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (string.IsNullOrEmpty((string)value))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }
    }
}
