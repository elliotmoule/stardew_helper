using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SV_VillagerHelper.Utilities
{
    public class ObservableProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
