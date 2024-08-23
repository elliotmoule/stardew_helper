using SV_VillagerHelper.Utilities;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SV_VillagerHelper.Converters
{
    public class SeasonToForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string season || string.IsNullOrEmpty(season))
            {
                return ColorHelper.GetBrushFromHex(null);
            }

            return season.ToLower() switch
            {
                "spring" => ColorHelper.GetBrushFromHex("#AAFF7F"),
                "summer" => ColorHelper.GetBrushFromHex("#FFD966"),
                "fall" => ColorHelper.GetBrushFromHex("#FF8C00"),
                "winter" => ColorHelper.GetBrushFromHex("#A4AFFF"),
                _ => ColorHelper.GetBrushFromHex(null),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush c)
            {
                return "#000000";
            }

            return ColorHelper.GetHexFromBrush(c) switch
            {
                "#AAFF7F" => "spring",
                "#FFD966" => "summer",
                "#FF8C00" => "fall",
                "#A4AFFF" => "winter",
                _ => "#000000"
            };
        }
    }
}
