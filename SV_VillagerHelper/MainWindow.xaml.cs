using SV_VillagerHelper.ViewModels;
using System.Windows;

namespace SV_VillagerHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AppViewModel viewModel;
        public MainWindow()
        {
            viewModel = new AppViewModel();
            DataContext = viewModel;
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _ = viewModel.LoadVillagersAsync();
            VillagerCombo.Focus();
        }
    }
}