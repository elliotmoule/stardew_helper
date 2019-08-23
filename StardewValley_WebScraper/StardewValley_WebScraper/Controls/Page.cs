using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValley_WebScraper.Controls
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
