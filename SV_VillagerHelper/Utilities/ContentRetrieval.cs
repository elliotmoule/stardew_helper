using HtmlAgilityPack;
using SV_VillagerHelper.Models;
using SV_VillagerHelper.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace SV_VillagerHelper.Utilities
{
    public static partial class ContentRetrieval
    {
        [GeneratedRegex(@"\r\n?|\n|\t")]
        private static partial Regex SV_VillagerSimplifyHtml();

        internal const string BASE_URL = @"https://stardewvalleywiki.com";

        public static async Task<ObservableCollection<Villager>> GetVillagersAsync(AppViewModel parent)
        {
            SynchronizationContext _syncContext = SynchronizationContext.Current;
            string villagerUrl = @$"{BASE_URL}/Villagers";
            using var httpClient = new HttpClient();
            var htmlDocument = new HtmlDocument();

            try
            {
                var html = await httpClient.GetStringAsync(villagerUrl);
                htmlDocument.LoadHtml(html);
            }
            catch (Exception)
            {
                throw;
            }

            var names = GetVillagerNamesAsync(htmlDocument);
            var links = RetrieveVillagerUrls(htmlDocument);
            var collection = new ObservableCollection<Villager>();
            parent.LoadingMaxValue = names.Count;

            ImageHelper.SaveVillagerAvatarsAsync(BASE_URL, names, links);

            var progress = new Progress<int>(value =>
            {
                parent.LoadingProgress = value;
            });

            var tasks = names.Select(async name =>
            {
                var trimmed = name.Trim();
                if (VillagerHelper.VillagerNameLinkMatch(links, trimmed, out var link))
                {
                    var villager = new Villager(name, $@"{BASE_URL}{link}");
                    villager.Avatar.AvatarFilePath = $"{Path.Combine(ImageHelper.BASE_FILE_LOCATION, trimmed)}.png";
                    collection.Add(villager);
                    links.Remove(link); // Make the list smaller each time.

                    await VillagerHelper.FillVillagerDetails(parent, httpClient, villager);

                    // Update progress on UI thread
                    _syncContext.Post(_ =>
                    {
                        parent.LoadingProgress++;
                    }, null);
                }
            });

            await Task.WhenAll(tasks);

            return collection;
        }

        static List<string> GetVillagerNamesAsync(HtmlDocument htmlDocument)
        {
            var names = new List<string>();

            try
            {
                var villagersHtmlNodes = htmlDocument.DocumentNode
                    .Descendants("ul")
                    .Where(node => node.GetAttributeValue("class", string.Empty)
                    .Equals("gallery mw-gallery-packed villagergallery"));

                foreach (var villagerHtml in villagersHtmlNodes)
                {
                    var innerText = SV_VillagerSimplifyHtml().Replace(villagerHtml.InnerText, "|");
                    var villagerNames = innerText
                        .Split('|', StringSplitOptions.RemoveEmptyEntries)
                        .Select(name => name.Replace(" ", "_"));

                    names.AddRange(villagerNames);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return names;
        }

        internal static async Task<MemoryStream> RetrieveWebImage(string url)
        {
            MemoryStream stream = null;
            if (string.IsNullOrWhiteSpace(url))
            {
                return stream;
            }

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var memoryStream = new MemoryStream();
                await using var imageStream = await response.Content.ReadAsStreamAsync();
                await imageStream.CopyToAsync(memoryStream);

                memoryStream.Position = 0; // Reset the position of the memory stream to the beginning

                return memoryStream;
            }
            catch (Exception)
            {
                throw;
            }
        }

        static List<string> RetrieveVillagerUrls(HtmlDocument htmlDocument)
        {
            var allLinks = new List<string>();
            if (htmlDocument == null)
            {
                return allLinks;
            }

            var villagersHtml = htmlDocument.DocumentNode
                .Descendants("div")
                .Where(node => node.GetAttributeValue("class", string.Empty)
                .Equals("thumb"))
                .ToList();

            foreach (var item in villagersHtml)
            {
                var link = item.Descendants("img")
                    .Select(node => node.GetAttributeValue("src", string.Empty))
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(link))
                {
                    continue;
                }

                allLinks.Add(link);
            }

            return allLinks;
        }
    }
}
