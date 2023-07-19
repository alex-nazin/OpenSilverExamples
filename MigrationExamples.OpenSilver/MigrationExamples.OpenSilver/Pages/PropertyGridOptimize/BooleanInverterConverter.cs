using System;
using System.Windows.Data;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !bool.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !bool.Parse(value.ToString());
        }
    }
}
