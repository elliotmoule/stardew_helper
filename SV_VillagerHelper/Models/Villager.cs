using SV_VillagerHelper.Utilities;
using System.Collections.ObjectModel;

namespace SV_VillagerHelper.Models
{
    public class Villager : ObservableProperties, IComparable<Villager>
    {
        public Villager(string name, string webUrl)
        {
            Name = name;
            Avatar.WebUrl = webUrl;
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyChanged(nameof(Name));
                NotifyChanged(nameof(DisplayName));
            }
        }

        public string DisplayName => Name.Replace("_", " ");

        private Gender _gender = Gender.NonBinary;
        public Gender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                NotifyChanged(nameof(Gender));
            }
        }

        private string _birthdaySeason = string.Empty;
        public string BirthdaySeason
        {
            get { return _birthdaySeason; }
            set
            {
                _birthdaySeason = value;
                NotifyChanged(nameof(BirthdaySeason));
            }
        }

        private int _birthday = 0;
        public int Birthday
        {
            get { return _birthday; }
            set
            {
                _birthday = value;
                NotifyChanged(nameof(Birthday));
            }
        }

        private string _livesIn = string.Empty;
        public string LivesIn
        {
            get { return _livesIn; }
            set
            {
                _livesIn = value;
                NotifyChanged(nameof(LivesIn));
            }
        }

        private string _address = string.Empty;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyChanged(nameof(Address));
            }
        }

        private bool _marriage = false;
        public bool Marriage
        {
            get { return _marriage; }
            set
            {
                _marriage = value;
                NotifyChanged(nameof(Marriage));
            }
        }

        private readonly ObservableCollection<FamilyMember> _familyMembers = [];
        public ObservableCollection<FamilyMember> FamilyMembers
        {
            get { return _familyMembers; }
        }

        private readonly ObservableCollection<string> _lovedGifts = [];
        public ObservableCollection<string> LovedGifts
        {
            get { return _lovedGifts; }
        }

        private readonly VillagerAvatar _avatar = new();
        public VillagerAvatar Avatar
        {
            get { return _avatar; }
        }

        private FamilyMember _selectedFamilyMember = null;
        public FamilyMember SelectedFamilyMember
        {
            get { return _selectedFamilyMember; }
            set
            {
                _selectedFamilyMember = value;
                NotifyChanged(nameof(SelectedFamilyMember));
            }
        }

        public int CompareTo(Villager other)
        {
            return string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
