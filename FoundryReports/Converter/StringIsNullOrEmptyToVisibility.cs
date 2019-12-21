using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FoundryReports.Converter
{
    internal class StringIsNullOrEmptyToVisibility : IValueConverter
    {
        private readonly bool _inverted;

        public static StringIsNullOrEmptyToVisibility EmptyIsVisible { get; } = new StringIsNullOrEmptyToVisibility(true);

        public static StringIsNullOrEmptyToVisibility EmptyIsCollapsed { get; } = new StringIsNullOrEmptyToVisibility(false);

        private StringIsNullOrEmptyToVisibility(bool inverted)
        {
            _inverted = inverted;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var asString = value?.ToString();
            var isEmpty = string.IsNullOrEmpty(asString);

            if (_inverted)
                isEmpty = !isEmpty;

            return isEmpty ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
