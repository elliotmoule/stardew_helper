using System.Globalization;
using System.Windows.Data;

namespace SV_VillagerHelper.Converters
{
    public class GenderToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not Gender gender
                ? null
                : gender switch
            {
                Gender.Male => "Male",
                Gender.Female => "Female",
                _ => "Non-Binary",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not string genderString
                ? null
                : genderString switch
            {
                "Male" => Gender.Male,
                "Female" => Gender.Female,
                _ => (object)Gender.NonBinary,
            };
        }
    }
}
