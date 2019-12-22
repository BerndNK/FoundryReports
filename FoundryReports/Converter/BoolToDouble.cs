using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundryReports.Converter
{
    internal class BoolToDouble : IValueConverter
    {
        public double ValueForTrue { get; set; } = 100;

        public double ValueForFalse { get; set; } = 0.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool asBool && asBool)
                return ValueForTrue;

            return ValueForFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
