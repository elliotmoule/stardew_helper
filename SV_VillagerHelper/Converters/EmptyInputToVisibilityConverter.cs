using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SV_VillagerHelper.Converters
{
    public class EmptyInputToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s))
            {
                if (string.Equals("Unknown", s, StringComparison.CurrentCultureIgnoreCase))
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
