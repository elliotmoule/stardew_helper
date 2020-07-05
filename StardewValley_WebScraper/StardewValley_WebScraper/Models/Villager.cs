using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using static StardewValley_WebScraper.Utilities.Enums;

namespace StardewValley_WebScraper.Models
{
    [Serializable]
    public class Villager : ObservableObject, IComparable
    {
        public Villager(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

        public Genders Gender { get; set; }

        public ImageSource Avatar { get; set; }

        internal string AvatarUrl { get; set; }

        public string Birthday { get; set; }

        public string LivesIn { get; set; }

        public string Address { get; set; }

        public string Marriage { get; set; }

        public List<string> BestGifts { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            Villager n1 = (Villager)obj;
            return string.Compare(this.Name, n1.Name);
        }
    }
}
