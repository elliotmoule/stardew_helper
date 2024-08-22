using SV_VillagerHelper.Utilities;
using SV_VillagerHelper.ViewModels;
using System.Windows.Input;

namespace SV_VillagerHelper.Models
{
    public class FamilyMember : ObservableProperties, IComparable<FamilyMember>
    {
        AppViewModel _parent;
        public FamilyMember(AppViewModel parent)
        {
            _parent = parent;
            FamilyMemberClickCommand = new RelayCommand<FamilyMember>(OnFamilyMemberClick);
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyChanged(nameof(Name));
            }
        }

        private string _relationship = string.Empty;
        public string Relationship
        {
            get { return _relationship; }
            set
            {
                _relationship = value;
                NotifyChanged(nameof(Relationship));
            }
        }

        private string _imagePath = string.Empty;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                NotifyChanged(nameof(ImagePath));
            }
        }

        public ICommand FamilyMemberClickCommand { get; set; }

        private void OnFamilyMemberClick(FamilyMember familyMember)
        {
            _parent.SelectVillager(familyMember.Name);
        }

        public int CompareTo(FamilyMember other)
        {
            return string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
