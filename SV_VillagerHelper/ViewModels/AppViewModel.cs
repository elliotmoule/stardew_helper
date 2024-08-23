using SV_VillagerHelper.Models;
using SV_VillagerHelper.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SV_VillagerHelper.ViewModels
{
    public class AppViewModel : ObservableProperties
    {
        public AppViewModel()
        {
            ReloadCommand = new RelayCommand<object>(async (input) =>
            {
                LoadingProgress = 0;
                Villagers.Clear();
                SelectedVillager = null;
                _currentlySelected = string.Empty;
                ImageHelper.ClearVillagerAvatars();
                await LoadVillagersAsync();
            });
        }

        private int _loadingMaxValue = 100;
        public int LoadingMaxValue
        {
            get { return _loadingMaxValue; }
            set
            {
                _loadingMaxValue = value;
                NotifyChanged(nameof(LoadingMaxValue));
                NotifyChanged(nameof(ProgressBarHeight));
            }
        }

        private int _loadingProgress = 0;
        public int LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                NotifyChanged(nameof(LoadingProgress));
                NotifyChanged(nameof(ProgressBarHeight));
            }
        }

        public GridLength ProgressBarHeight => new(LoadingProgress == LoadingMaxValue ? 6 : 15, GridUnitType.Pixel);

        private ObservableCollection<Villager> _villagers = [];
        public ObservableCollection<Villager> Villagers
        {
            get { return _villagers; }
            set
            {
                _villagers = value;
                NotifyChanged(nameof(Villagers));
            }
        }

        private Villager _selectedVillager = null;
        public Villager SelectedVillager
        {
            get { return _selectedVillager; }
            set
            {
                _selectedVillager = value;
                NotifyChanged(nameof(SelectedVillager));
            }
        }

        public ICommand ReloadCommand { get; set; }

        internal async Task LoadVillagersAsync()
        {
            var villagers = await ContentRetrieval.GetVillagersAsync(this);
            Villagers.Clear();
            Villagers.AddRange(villagers);

            if (Villagers.Count > 0)
            {
                SelectedVillager = Villagers[0];
            }
        }

        string _currentlySelected = string.Empty;

        internal void SelectVillager(string name)
        {
            if (_currentlySelected == name)
            {
                var villager = Villagers.FirstOrDefault(x => x.Name == name);

                if (villager != null)
                {
                    SelectedVillager = villager;
                }

                _currentlySelected = string.Empty;
            }
            else
            {
                _currentlySelected = name;
            }
        }
    }
}
