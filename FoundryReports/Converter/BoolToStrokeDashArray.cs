using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FoundryReports.Converter
{
    internal class BoolToStrokeDashArray : IValueConverter
    {
        public static BoolToStrokeDashArray Instance { get; } = new BoolToStrokeDashArray();

        private BoolToStrokeDashArray()
        {
            
        }

        private DoubleCollection Dashed { get; } = new DoubleCollection(new [] {1, 1.0});

        private DoubleCollection Continuous { get; } = new DoubleCollection(new [] {1.0, 0.0});

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool asBool && asBool)
            {
                return Dashed;
            }

            return Continuous;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
