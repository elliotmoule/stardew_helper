using StardewValley_WebScraper.Controls;
using StardewValley_WebScraper.Models;
using StardewValley_WebScraper.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StardewValley_WebScraper.Views
{
    /// <summary>
    /// Interaction logic for CharactersView.xaml
    /// </summary>
    public partial class CharactersView : UserControl
    {
        CharactersViewModel vm;
        bool firstLoad = false;
        bool componentLoaded = false;

        public CharactersView()
        {
            vm = new CharactersViewModel();
            DataContext = vm;
            InitializeComponent();
            SavedText.Text = "Loading";
            SavedText.Visibility = Visibility.Visible;
        }

        private void VillagersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (componentLoaded)
            {
                LoadVillagerData();
            }
        }

        private bool LoadVillagerData()
        {
            Villager villager = vm.Villagers.Where(n => n.Name == VillagerNameText.Text).FirstOrDefault();
            VillagerNameText.Foreground = villager.Gender == Utilities.Enums.Genders.Male ? Brushes.CornflowerBlue : (villager.Gender == Utilities.Enums.Genders.Unknown ? Brushes.Black : Brushes.LightPink);
            VillagerImage.Source = villager.Avatar != null ? villager.Avatar : Utilities.Images.BitmapToImageSource(Utilities.Images.CreateDefaultAvatar());

            if (!string.IsNullOrWhiteSpace(villager.Birthday))
            {
                VillagerInfo_1.Children.Clear();
                if (villager.Gender != Utilities.Enums.Genders.Unknown)
                {
                    VillagerInfo_1.Children.Add(new TextBlock { Text = $"Gender: {villager.Gender.ToString()}", Margin = new Thickness(0, 5, 0, 5), TextAlignment = TextAlignment.Center });
                }
                if (!villager.Birthday.Contains("Unknown"))
                {
                    VillagerInfo_1.Children.Add(new TextBlock { Text = $"Birthday: {villager.Birthday}", Margin = new Thickness(0, 5, 0, 5), TextAlignment = TextAlignment.Center });
                }
                if (!villager.LivesIn.Contains("N/A") && !string.IsNullOrWhiteSpace(villager.LivesIn))
                {
                    VillagerInfo_1.Children.Add(new TextBlock { Text = $"{villager.LivesIn}", Margin = new Thickness(0, 5, 0, 5), TextAlignment = TextAlignment.Center });
                }
                if (!villager.Address.Contains("N/A") && !string.IsNullOrWhiteSpace(villager.Address.Replace("Address:", "")))
                {
                    VillagerInfo_1.Children.Add(new TextBlock { Text = $"{villager.Address}", Margin = new Thickness(0, 5, 0, 5), TextAlignment = TextAlignment.Center, TextWrapping = TextWrapping.Wrap });
                }
                VillagerInfo_1.Children.Add(new TextBlock { Text = $"{villager.Marriage}", Margin = new Thickness(0, 5, 0, 5), TextAlignment = TextAlignment.Center, TextWrapping = TextWrapping.Wrap });
                VillagerInfo_2.Children.Clear();
                if (villager.BestGifts != null && villager.BestGifts.Count > 0)
                {
                    BestGiftText.Visibility = Visibility.Visible;
                    foreach (var item in villager.BestGifts)
                    {
                        VillagerInfo_2.Children.Add(new TextBlock { Text = $"   {item}", Margin = new Thickness(0, 1, 0, 1), TextAlignment = TextAlignment.Center, Foreground = Brushes.ForestGreen });
                    }
                }
                else
                {
                    BestGiftText.Visibility = Visibility.Hidden;
                }
            }

            return true;
        }

        private void ProgressBarElement_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            SavedText.Visibility = e.NewValue > 1 ? Visibility.Hidden : Visibility.Visible;

            if ((e.NewValue > 50 && firstLoad == false))
            {
                firstLoad = true;
                LoadVillagerData();
            }

            if (ProgressBarElement.Value == ProgressBarElement.Maximum)
            {
                bool success = Utilities.Saving.Save(vm.Villagers);
                SavedText.Text = success ? "Saved" : "Up-to-date";
                SavedText.Visibility = Visibility.Visible;
                ProgressBarElement.Visibility = Visibility.Hidden;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            componentLoaded = true;
        }
    }
}
