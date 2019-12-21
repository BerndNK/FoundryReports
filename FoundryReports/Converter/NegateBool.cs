using System;
using System.Globalization;
using System.Windows.Data;

namespace FoundryReports.Converter
{
    internal class NegateBool : IValueConverter
    {
        public static NegateBool Instance { get; }= new NegateBool();

        private NegateBool()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool asBool)
                return !asBool;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
