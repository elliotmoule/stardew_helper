using HtmlAgilityPack;
using SV_VillagerHelper.Models;
using SV_VillagerHelper.ViewModels;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SV_VillagerHelper.Utilities
{
    public static partial class VillagerHelper
    {
        private static readonly char[] paranthesisSeparator = ['(', ')'];
        private static readonly char[] spaceSeparator = [' '];
        private static readonly string[] addressSeparator = ["Address"];

        #region Generated Regex Patterns
        [GeneratedRegex("Bachelorette", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex GenderPattern();

        [GeneratedRegex(@"\s+|&#160;")]
        private static partial Regex HtmlReplacePattern();

        [GeneratedRegex(@"(Spring|Summer|Fall|Winter)\s(\d+)")]
        private static partial Regex SeasonReplacementPattern();

        [GeneratedRegex(@"&lt;")]
        private static partial Regex LessThanSymbolPattern();
        [GeneratedRegex(@"≥")]
        private static partial Regex GreaterThanSymbolPattern();

        [GeneratedRegex(@"(?<=\))\s+(?=\w)")]
        private static partial Regex NewLineCheckPattern();
        #endregion

        internal static bool VillagerNameLinkMatch(List<string> links, string name, out string matchedLink)
        {
            matchedLink = string.Empty;
            for (var i = links.Count; i-- > 0;)
            {
                if (links[i].Contains(name))
                {
                    matchedLink = links[i];
                    return true;
                }
            }

            return false;
        }

        internal static async Task FillVillagerDetails(AppViewModel parent, HttpClient httpClient, Villager villager)
        {
            var url = @$"{ContentRetrieval.BASE_URL}/{villager.Name}";

            try
            {
                var html = await httpClient.GetStringAsync(url);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var villagerHtml = htmlDocument.DocumentNode
                    .Descendants("table")
                    .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty) == "infoboxtable");

                if (villagerHtml == null)
                {
                    return;
                }

                foreach (var tableRow in villagerHtml.Descendants("tr"))
                {
                    string text = tableRow.InnerText.Trim();
                    switch (StringHelper.GetTableRow(text, "Birthday", "Lives In", "Address", "Family", "Marriage", "Loved Gifts"))
                    {
                        case "Birthday":
                            FillBirthdayDetails(ref villager, text);
                            continue;
                        case "Lives In":
                            villager.LivesIn = GetLivedInLocation(text);
                            continue;
                        case "Address":
                            villager.Address = GetAddress(text);
                            continue;
                        case "Family":
                            FillFamilyDetails(ref villager, tableRow, parent);
                            continue;
                        case "Marriage":
                            villager.Marriage = GetMarriage(text);
                            continue;
                        case "Loved Gifts":
                            FillLovedGiftsDetails(ref villager, tableRow);
                            continue;
                    }
                }

                if (villager.Gender == Gender.NonBinary)
                {
                    villager.Gender = ExtractGender(htmlDocument, villager.Marriage);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        static void FillBirthdayDetails(ref Villager villager, string text)
        {
            var info = ExtractBirthday(text);
            if (info == null || info.Length < 2)
            {
                return;
            }

            villager.BirthdaySeason = info[0];
            villager.Birthday = int.TryParse(info[1], out var date) ? date : 0;
        }

        public static string GetLivedInLocation(string input)
        {
            // Remove unwanted characters (like newlines and HTML entities)
            string cleanedInput = HtmlReplacePattern().Replace(input, " ").Trim();

            // Remove the prefix ("Lives In") and trim the remaining part
            string location = cleanedInput.Replace("Lives In", "").Trim();

            return location;
        }

        public static string GetAddress(string input)
        {
            // Replace HTML entities with their respective characters
            string cleanedInput = LessThanSymbolPattern().Replace(input, "<");
            cleanedInput = GreaterThanSymbolPattern().Replace(cleanedInput, "≥");
            cleanedInput = HtmlReplacePattern().Replace(cleanedInput, " ").Trim();

            // Remove the "Address" prefix and trim the remaining part
            string addressPart = cleanedInput.Replace("Address", "").Trim();

            // Use regex to split the address into parts that should be on separate lines
            string[] multiLineParts = NewLineCheckPattern().Split(addressPart);

            // Join the parts with newline characters
            string finalAddress = string.Join("\n", multiLineParts);
            return finalAddress;
        }

        static void FillFamilyDetails(ref Villager villager, HtmlNode htmlNode, AppViewModel parent)
        {
            var familyMembers = htmlNode.Descendants("p")
                .Select(p => ExtractFamilyMemberDetails(p, parent))
                .Where(member => member != null)
                .ToList();

            foreach (var member in familyMembers)
            {
                if (member.Relationship.Contains("wife", StringComparison.OrdinalIgnoreCase))
                {
                    villager.Gender = Gender.Male;
                    break;
                }
                else if (member.Relationship.Contains("husband", StringComparison.OrdinalIgnoreCase))
                {
                    villager.Gender = Gender.Female;
                    break;
                }
            }

            if (familyMembers.Count != 0)
            {
                villager.FamilyMembers.AddRange(familyMembers);
            }
        }

        public static bool GetMarriage(string input)
        {
            // Remove unwanted characters (like newlines and HTML entities)
            string cleanedInput = HtmlReplacePattern().Replace(input, " ").Trim();

            // Remove the prefix ("Lives In") and trim the remaining part
            string location = cleanedInput.Replace("Marriage", "").Trim();

            return location == "Yes";
        }

        static void FillLovedGiftsDetails(ref Villager villager, HtmlNode htmlNode)
        {
            var gifts = htmlNode.Descendants("a")
                .Select(gift => gift.InnerText)
                .ToList();

            if (gifts.Count != 0)
            {
                villager.LovedGifts.AddRange(gifts);
            }
        }

        static Gender ExtractGender(HtmlDocument htmlDocument, bool isMarried)
        {
            if (isMarried && GenderPattern().Matches(htmlDocument.DocumentNode.InnerHtml).Count is int count)
            {
                return count > 1 ? Gender.Female : Gender.Male;
            }

            var divNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']");

            if (divNode == null)
            {
                return Gender.NonBinary;
            }

            var summary = divNode.Descendants("p")
                .FirstOrDefault(p => p.Descendants("b").Any());

            if (summary != null)
            {
                var lowercase = summary.InnerText.ToLower();

                if (lowercase.ContainsAny("she", "woman"))
                {
                    return Gender.Female;
                }
                else if (lowercase.ContainsAny("he", "man"))
                {
                    return Gender.Male;
                }
                else if (lowercase.Contains("marnie", StringComparison.OrdinalIgnoreCase))
                {
                    return Gender.Female;
                }
                else if (lowercase.ContainsAny("gunther", "linus", "wizard"))
                {
                    return Gender.Male;
                }
            }

            return Gender.NonBinary;
        }

        static FamilyMember ExtractFamilyMemberDetails(HtmlNode pNode, AppViewModel parent)
        {
            try
            {
                // Extracting the image path
                var imgNode = pNode.SelectSingleNode(".//img");
                var imgPath = imgNode?.GetAttributeValue("src", string.Empty);

                // Extracting the name
                var nameNode = pNode.SelectSingleNode(".//a[@title]");
                var name = nameNode?.InnerText.Trim();

                // Extracting the relationship information (e.g., "Wife", "Son")
                var relationshipText = pNode.InnerText;
                var relationship = relationshipText
                    .Split(paranthesisSeparator, StringSplitOptions.RemoveEmptyEntries)
                    .LastOrDefault()?
                    .Trim();

                return new FamilyMember(parent) { Name = name, Relationship = relationship, ImagePath = @$"{ContentRetrieval.BASE_URL}/{imgPath}" };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string[] ExtractBirthday(string input)
        {
            // Remove unwanted characters (like newlines and HTML entities)
            string cleanedInput = HtmlReplacePattern().Replace(input, " ").Trim();

            // Use regex to extract the season and the day
            var match = SeasonReplacementPattern().Match(cleanedInput);

            if (match.Success)
            {
                return [match.Groups[1].Value, match.Groups[2].Value];
            }

            return []; // Return an empty array if no match found
        }
    }
}
