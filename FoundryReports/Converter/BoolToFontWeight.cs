using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FoundryReports.Converter
{
    internal class BoolToFontWeight : IValueConverter
    {
        public FontWeight FontWeightForTrue { get; set; } = FontWeights.Bold;

        public FontWeight FontWeightForFalse { get; set; } = FontWeights.Regular;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool asBool && asBool)
                return FontWeightForTrue;

            return FontWeightForFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
