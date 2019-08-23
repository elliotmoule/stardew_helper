using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace WebScraper_Text
{
    class Program
    {
        static void Main(string[] args)
        {
            GetHtmlAsync();
            Console.ReadLine();
        }

        public static async void GetHtmlAsync()
        {
            var url = "https://stardewvalleywiki.com/Villagers";
            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var villagersHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("gallery mw-gallery-packed villagergallery")).ToList();

            List<string> villagers = new List<string>();

            foreach (var item in villagersHtml)
            {
                string innerText = item.InnerText;
                innerText = Regex.Replace(innerText, @"\r\n?|\n|\t", "|");
                bool name = false;
                string newName = string.Empty;
                foreach (char letter in innerText)
                {
                    if(letter != '|')
                    {
                        name = true;
                        newName += letter;
                    }
                    else
                    {
                        if (name == true)
                        {
                            name = false;
                            villagers.Add(newName);
                            Console.WriteLine(newName);
                            newName = string.Empty;
                        }
                    }
                }
            }

            Console.WriteLine();

        }
    }
}
