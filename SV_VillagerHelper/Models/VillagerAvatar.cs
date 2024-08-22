using SV_VillagerHelper.Utilities;
using System.Windows.Media;

namespace SV_VillagerHelper.Models
{
    public class VillagerAvatar : ObservableProperties
    {
        public ImageSource Image => ImageHelper.LoadImage(AvatarFilePath);

        private string _avatarFilePath = string.Empty;
        public string AvatarFilePath
        {
            get { return _avatarFilePath; }
            set
            {
                _avatarFilePath = value;
                NotifyChanged(nameof(AvatarFilePath));
                NotifyChanged(nameof(Image));
            }
        }

        public string WebUrl { get; set; } = string.Empty;
    }
}
