using System.Text.RegularExpressions;

namespace SV_VillagerHelper.Utilities
{
    public static partial class StringHelper
    {
        public static bool ContainsAny(this string source, params string[] targets) => targets.Any(word => Regex.IsMatch(source, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase));

        /// <summary>
        /// Runs a Contains check against the <paramref name="source"/> looking for any of the given <paramref name="targets"/>.
        /// </summary>
        /// <param name="source">The text source to check within.</param>
        /// <param name="targets">The target strings to use to check for a Contains match.</param>
        /// <returns>Returns the matched string from <paramref name="targets"/>.</returns>
        public static string GetTableRow(string source, params string[] targets)
        {
            var target = targets.FirstOrDefault(word => Regex.IsMatch(source, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase));

            return target ?? string.Empty;
        }
    }
}
