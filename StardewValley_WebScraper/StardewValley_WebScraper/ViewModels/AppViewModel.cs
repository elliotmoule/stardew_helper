using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using StardewValley_WebScraper.Models;

namespace StardewValley_WebScraper.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public AppViewModel()
        {
            const string removeString1 = "StardewValley_WebScraper.Views.";
            const string removeString2 = "View";

            IList<Page> pages = new List<Page>();
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == "StardewValley_WebScraper.Views"
                    select t;
            q.ToList().ForEach(t =>
            {
                string name = t.ToString();
                int index = name.IndexOf(removeString1);
                string cleanName1 = (index < 0) ? name : name.Remove(index, removeString1.Length);
                index = cleanName1.IndexOf(removeString2);
                string cleanName2 = (index < 0) ? cleanName1 : cleanName1.Remove(index, removeString2.Length);
                pages.Add(new Page(cleanName2));
            });
            _pages = new CollectionView(pages);
        }

        private readonly CollectionView _pages;
        private string _page;

        public CollectionView Pages
        {
            get { return _pages; }
        }

        public string CurrentPage
        {
            get { return _page; }
            set
            {
                if (_page == value)
                {
                    return;
                }
                _page = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
