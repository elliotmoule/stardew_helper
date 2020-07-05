using HtmlAgilityPack;
using StardewValley_WebScraper.Models;
using StardewValley_WebScraper.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StardewValley_WebScraper.ViewModels
{
    public class CharactersViewModel : INotifyPropertyChanged
    {
        public CharactersViewModel()
        {
            try
            {
                KeyValuePair<bool, bool> accessKeyValue = Saving.CanAccessSaveLoad;
                if (accessKeyValue.Key)
                {
                    if (accessKeyValue.Value)
                    {
                        _villagers = Saving.Load();
                    }

                    if (Villagers == null)
                    {
                        GetVillagersAsync();
                    }
                    else
                    {
                        Villager = Villagers[0].Name;
                        LoadVillagerImagesLocalAsync();
                    }
                }
                else
                {                    
                    Villager = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    _villagers = new ObservableCollection<Villager>();
                    Villager Villager_InTest = new Villager(Villager);
                    
                    _villagers.Add(Villager_InTest);
                    OnPropertyChanged("Villagers");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ObservableCollection<Villager> _villagers;
        private string _villager;
        private int _villagerNumber = 0;
        private int _loadingProgress = 0;

        public ObservableCollection<Villager> Villagers
        {
            get { return _villagers; }
        }

        public int VillagerNumber
        {
            get { return _villagerNumber; }
            set
            {
                if (_villagerNumber == value)
                {
                    return;
                }
                _villagerNumber = value;
                OnPropertyChanged("VillagerNumber");
            }
        }

        public int LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                if (_loadingProgress == value)
                {
                    return;
                }
                _loadingProgress = value;
                OnPropertyChanged("LoadingProgress");
            }
        }

        public string Villager
        {
            get { return _villager; }
            set
            {
                if (_villager == value)
                {
                    return;
                }
                _villager = value;
                OnPropertyChanged("Villager");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void GetVillagersAsync()
        {
            var url = "https://stardewvalleywiki.com/Villagers";
            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var villagersHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("gallery mw-gallery-packed villagergallery")).ToList();

            IList<Villager> villagers = new List<Villager>();
            foreach (var item in villagersHtml)
            {
                string innerText = item.InnerText;
                innerText = Regex.Replace(innerText, @"\r\n?|\n|\t", "|");
                bool name = false;
                string newName = string.Empty;
                foreach (char letter in innerText)
                {
                    if (letter != '|')
                    {
                        name = true;
                        newName += letter;
                    }
                    else
                    {
                        if (name == true)
                        {
                            name = false;
                            villagers.Add(new Villager(newName));
                            newName = string.Empty;
                        }
                    }
                }
            }
            _villagers = new ObservableCollection<Villager>(villagers);
            _villagers = new ObservableCollection<Villager>(_villagers.OrderBy(i => i));
            OnPropertyChanged("Villagers");
            VillagerNumber = _villagers.Count * 2;
            LoadingProgress = 0;
            LoadVillagerInformation();
            LoadVillagerImagesAsync(htmlDocument);
        }

        public async void LoadVillagerImagesAsync(HtmlDocument htmlDocument)
        {
            if (htmlDocument == null) return;
            string baseUrl = "https://stardewvalleywiki.com";

            var villagersHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("thumb")).ToList();

            foreach (var item in villagersHtml)
            {
                var link = item.Descendants("img")
                    .Where(node => node.GetAttributeValue("src", "")
                    .Contains("/mediawiki"))
                    .FirstOrDefault()
                    .ChildAttributes("src")
                    .FirstOrDefault()
                    .Value;

                foreach (var villager in Villagers)
                {
                    if (villager != null && !string.IsNullOrWhiteSpace(villager.Name))
                    {
                        string nameToTest = villager.Name.ToLower();
                        if (nameToTest.Contains("qi"))
                        {
                            nameToTest = "mr._qi";
                        }
                        if (link.ToLower().Contains(nameToTest))
                        {
                            string url = baseUrl;
                            url += link;
                            villager.Avatar = await Images.LoadImage(url, villager);
                            LoadingProgress++;
                        }
                    }
                }
            }

            OnPropertyChanged("Villagers");
        }

        public async void LoadVillagerImagesLocalAsync()
        {
            VillagerNumber = Villagers.Count * 2;
            LoadingProgress = 0;
            foreach (var villager in Villagers)
            {
                villager.Avatar = await Images.LoadImage(villager.Name);
                LoadingProgress += 2;
            }

            OnPropertyChanged("Villagers");
        }

        public void LoadVillagerInformation()
        {
            Parallel.ForEach(Villagers, async (villager) =>
            {
                string villagerName = villager.Name.ToLower().Contains("qi") ? "Mr._Qi" : villager.Name;
                var url = "https://stardewvalleywiki.com/" + villagerName;
                var httpClient = new HttpClient();

                var html = await httpClient.GetStringAsync(url);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var villagerHtml = htmlDocument.DocumentNode.Descendants("table")
                    .Where(node => node.GetAttributeValue("id", "")
                    .Equals("infoboxtable"))
                    .FirstOrDefault();

                // Birthday

                var birthdayHtml = villagerHtml.Descendants("td")
                    .Where(node => node.GetAttributeValue("id", "")
                    .Equals("infoboxdetail"))
                    .FirstOrDefault();

                string weather = "Unknown";
                string date = "";

                if (birthdayHtml.ChildNodes[0].ChildNodes.Count > 1)
                {
                    weather = birthdayHtml
                        .ChildNodes[0]?.ChildNodes[2]?.InnerText.Trim();

                    date = birthdayHtml
                        .ChildNodes[1]?.InnerText?.Trim('\n').Trim();
                }

                villager.Birthday = $"{weather} {date}";

                // Gender | Marriage

                var table = villagerHtml.Descendants("tr");

                foreach (var item in table)
                {
                    var text = item.InnerText;
                    if (text.Contains("Marriage"))
                    {
                        villager.Marriage = Regex.Replace(text, @"\t|\n|\r", "").Replace(":", ": ").Trim();
                    }
                    else if (text.Contains("Lives In"))
                    {
                        villager.LivesIn = Regex.Replace(text, @"\t|\n|\r", "").Replace(":", ": ").Trim();
                    }
                    else if (text.Contains("Address"))
                    {
                        villager.Address = Regex.Replace(text, @"\t|\n|\r", "").Replace(":", ": ").Trim();
                    }
                    else if (text.Contains("Best Gifts"))
                    {
                        var gifts = item.Descendants("a").ToList();
                        if (gifts.Count > 0 && villager.BestGifts == null)
                        {
                            villager.BestGifts = new List<string>();
                        }

                        foreach (var gift in gifts)
                        {
                            villager.BestGifts.Add(gift.InnerText);
                        }
                    }
                }

                if (villager.Marriage.Contains("Yes"))
                {
                    int count = 0, n = 0;
                    while ((n = htmlDocument.DocumentNode.InnerHtml.IndexOf("Bachelorette", n, StringComparison.InvariantCultureIgnoreCase)) != -1)
                    {
                        n += "Bachelorette".Length;
                        ++count;
                    }

                    villager.Gender = count > 1 ? Enums.Genders.Female : Enums.Genders.Male;
                }

                LoadingProgress++;
            });
            //foreach (var villager in Villagers)
            //{

            //}
        }
    }
}
