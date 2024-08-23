using System.Windows.Media;

namespace SV_VillagerHelper.Utilities
{
    public static class ColorHelper
    {
        /// <summary>
        /// Creates a <see cref="SolidColorBrush"/> from hex code.
        /// </summary>
        /// <param name="hexColor">The string Hex Code. Does not need to be prepended with '#'.</param>
        /// <returns>Returns a <see cref="SolidColorBrush"/> for a given Hex Code. If the input is <see cref="null"/> then <see cref="Brushes.Black"/> will be returned.</returns>
        public static SolidColorBrush GetBrushFromHex(string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor))
            {
                return Brushes.Black;
            }

            // Ensure the hex color code starts with a '#'
            if (!hexColor.StartsWith('#'))
            {
                hexColor = "#" + hexColor;
            }

            // Convert the hex color to a Color object
            Color color = (Color)ColorConverter.ConvertFromString(hexColor);

            // Create and return a SolidColorBrush based on the Color object
            return new SolidColorBrush(color);
        }

        public static string GetHexFromBrush(SolidColorBrush brush)
        {
            // Get the Color object from the SolidColorBrush
            Color color = brush.Color;

            // Convert the Color object to a hex string
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
