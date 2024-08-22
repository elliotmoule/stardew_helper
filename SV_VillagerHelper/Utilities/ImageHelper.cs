using System.IO;
using System.Windows.Media.Imaging;

namespace SV_VillagerHelper.Utilities
{
    public static class ImageHelper
    {
        internal static readonly string BASE_FILE_LOCATION = Path.Combine(".", "Villagers");

        internal static BitmapImage LoadImage(string filePath)
        {
            var bitmap = new BitmapImage();

            try
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            catch (Exception)
            {
                throw;
            }

            return bitmap;
        }

        internal static void SaveVillagerAvatarsAsync(string baseUrl, List<string> names, List<string> links)
        {
            if (names == null || Directory.Exists(BASE_FILE_LOCATION))
            {
                return;
            }

            Directory.CreateDirectory(BASE_FILE_LOCATION);

            var tasks = new List<Task>();
            foreach (var name in names)
            {
                var filePath = $"{Path.Combine(BASE_FILE_LOCATION, name)}.png";
                if (!VillagerHelper.VillagerNameLinkMatch(links, name, out var link))
                {
                    continue;
                }

                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    var stream = await ContentRetrieval.RetrieveWebImage($@"{baseUrl}{link}");
                    await SaveStreamToFileAsync(stream, filePath);
                }));
            }

            Task.WaitAll([.. tasks]);
        }

        static async Task SaveStreamToFileAsync(MemoryStream memoryStream, string filePath)
        {
            try
            {
                memoryStream.Position = 0;

                await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await memoryStream.CopyToAsync(fileStream);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
