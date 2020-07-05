namespace StardewValley_WebScraper.Models
{
    public class Page
    {
        public string Name { get; set; }

        public object Content { get; set; }

        public Page(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
