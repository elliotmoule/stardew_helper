using SV_VillagerHelper.Models;
using SV_VillagerHelper.Utilities;
using System.Collections.ObjectModel;

namespace SV_VillagerHelper.ViewModels
{
    public class AppViewModel : ObservableProperties
    {
        private int _loadingMaxValue = 100;
        public int LoadingMaxValue
        {
            get { return _loadingMaxValue; }
            set
            {
                _loadingMaxValue = value;
                NotifyChanged(nameof(LoadingMaxValue));
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
            }
        }

        private ObservableCollection<Villager> _villagers = new();
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
