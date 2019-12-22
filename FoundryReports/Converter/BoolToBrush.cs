using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FoundryReports.Converter
{
    internal class BoolToBrush : IValueConverter
    {
        public Brush BrushForTrue { get; set; } = new SolidColorBrush();

        public Brush BrushForFalse { get; set; } = new SolidColorBrush();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool asBool && asBool)
                return BrushForTrue;

            return BrushForFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
