using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FoundryReports.Converter
{
    internal class BoolToVisibility : IValueConverter
    {
        private readonly bool _inverted;

        public static BoolToVisibility Normal { get; } = new BoolToVisibility(false);

        public static BoolToVisibility Inverted { get; } = new BoolToVisibility(true);

        private BoolToVisibility(bool inverted)
        {
            _inverted = inverted;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool asBool)
            {
                if (_inverted)
                    asBool = !asBool;

                return asBool ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
