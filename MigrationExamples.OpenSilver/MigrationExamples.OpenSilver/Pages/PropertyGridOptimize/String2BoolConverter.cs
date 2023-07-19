using System;
using System.Windows.Data;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public class String2BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string boolValue)
            {
                if (boolValue == "")
                    return null;
                else
                    return boolValue == "1";
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return bool.Parse(value.ToString()) ? "1" : "0";
        }
    }
}
