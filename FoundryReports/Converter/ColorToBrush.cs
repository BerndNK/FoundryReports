using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FoundryReports.Converter
{
    internal class ColorToBrush : IValueConverter
    {
        public static ColorToBrush Instance { get;} = new ColorToBrush();

        private ColorToBrush()
        {
            
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Color asColor)
                return new SolidColorBrush(asColor);

            return new SolidColorBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
