using StardewValley_WebScraper.ViewModels;
using System.Windows.Controls;

namespace StardewValley_WebScraper.Controls
{
    /// <summary>
    /// Interaction logic for Navbar.xaml
    /// </summary>
    public partial class Navbar : UserControl
    {
        public Navbar()
        {
            AppViewModel vm = new AppViewModel();
            DataContext = vm;
            InitializeComponent();
        }
    }
}
