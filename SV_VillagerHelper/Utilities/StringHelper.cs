using System.Text.RegularExpressions;

namespace SV_VillagerHelper.Utilities
{
    public static partial class StringHelper
    {
        private static readonly char[] separator = ['\n'];
        [GeneratedRegex(@"\t|\r")]
        private static partial Regex ReplaceTabsAndCarriageReturnPattern();
        [GeneratedRegex(@"\n+")]
        private static partial Regex ReplaceNewLinePattern();

        public static string FormatText(string text)
        {
            // Remove newlines, tabs, and excess whitespace
            text = ReplaceTabsAndCarriageReturnPattern().Replace(text, "").Trim();
            text = ReplaceNewLinePattern().Replace(text, "\n"); // Replace multiple newlines with a single newline

            // Identify the position of the first newline and extract the text after it
            var parts = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            return parts.Length > 1 ? parts[1].Trim() : string.Empty;
        }

        public static bool ContainsAny(this string source, params string[] targets) => targets.Any(word => Regex.IsMatch(source, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase));
    }
}
