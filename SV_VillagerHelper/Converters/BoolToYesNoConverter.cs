using System.Globalization;
using System.Windows.Data;

namespace SV_VillagerHelper.Converters
{
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b ? (b ? "Yes" : "No") : "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string yesNo && yesNo.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }
    }
}
