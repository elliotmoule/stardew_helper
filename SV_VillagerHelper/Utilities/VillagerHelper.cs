using HtmlAgilityPack;
using SV_VillagerHelper.Models;
using SV_VillagerHelper.ViewModels;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SV_VillagerHelper.Utilities
{
    public static partial class VillagerHelper
    {
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

                villager.Birthday = GetBirthday(villagerHtml);

                foreach (var item in villagerHtml.Descendants("tr"))
                {
                    string text = item.InnerText.Trim();
                    if (text.Contains("Birthday", StringComparison.OrdinalIgnoreCase))
                    {
                        villager.Birthday = StringHelper.FormatText(text);
                    }
                    else if (text.Contains("Lives In", StringComparison.OrdinalIgnoreCase))
                    {
                        villager.LivesIn = StringHelper.FormatText(text);
                    }
                    else if (text.Contains("Address", StringComparison.OrdinalIgnoreCase))
                    {
                        villager.Address = StringHelper.FormatText(text);
                    }
                    else if (text.Contains("Family", StringComparison.OrdinalIgnoreCase))
                    {
                        var familyMembers = item.Descendants("p")
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
                    else if (text.Contains("Marriage", StringComparison.OrdinalIgnoreCase))
                    {
                        villager.Marriage = StringHelper.FormatText(text) == "Yes";
                    }
                    else if (text.Contains("Loved Gifts", StringComparison.OrdinalIgnoreCase))
                    {
                        var gifts = item.Descendants("a")
                            .Select(gift => gift.InnerText)
                            .ToList();

                        if (gifts.Count != 0)
                        {
                            villager.LovedGifts.AddRange(gifts);
                        }
                    }
                }

                if (villager.Gender == Gender.NonBinary)
                {
                    villager.Gender = ExtractGender(htmlDocument, villager.Marriage);
                }
            }
            catch (Exception ex)
            {
                throw;
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

        private static readonly char[] separator = ['(', ')'];

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
                    .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                    .LastOrDefault()?
                    .Trim();

                return new FamilyMember(parent) { Name = name, Relationship = relationship, ImagePath = @$"{ContentRetrieval.BASE_URL}/{imgPath}" };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        static string GetBirthday(HtmlNode villagerNode)
        {
            var birthday = string.Empty;
            var birthdayHtml = villagerNode.Descendants("td")
                    .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty) == "infoboxdetail");

            if (birthdayHtml != null && birthdayHtml.ChildNodes.Count > 1)
            {
                string weather = birthdayHtml.ChildNodes[0].ChildNodes.Count > 1 && birthdayHtml.ChildNodes[0].ChildNodes[2].InnerText is string weatherInner ? weatherInner.Trim() : "Unknown";
                string date = birthdayHtml.ChildNodes[1].InnerText is string dateInner && dateInner.Trim('\n').Trim() is string dateTrimmed ? dateTrimmed : string.Empty;
                birthday = $"{weather} {date}";
            }

            return birthday;
        }

        [GeneratedRegex("Bachelorette", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex GenderPattern();
    }
}
