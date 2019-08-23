using StardewValley_WebScraper.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StardewValley_WebScraper.Utilities
{
    public static class Images
    {
        const string baseFilePath = @".\VillagerImages\";
        const string extension = @".png";

        public static async Task<ImageSource> LoadImage(string villagerName)
        {
            Image image = null;
            string filename = baseFilePath + villagerName + extension;
            await Task.Run(() =>
            {
                image = Image.FromFile(filename);
            });

            if (image == null)
            {
                image = CreateDefaultAvatar();
            }

            return GetImageStream(image);
        }

        public static async Task<ImageSource> LoadImage(string url, Villager villager)
        {
            Image image = null;
            string filename = baseFilePath + villager.Name + extension;
            await Task.Run(async () =>
            {
                if (!File.Exists(filename))
                {
                    await SaveImageAsync(url, villager);
                }

                image = Image.FromFile(filename);
            });

            if (image == null)
            {
                image = CreateDefaultAvatar();
            }

            return GetImageStream(image);
        }

        public static async Task<Bitmap> SaveImageAsync(string url, Villager villager)
        {
            string filename = baseFilePath + villager.Name + extension;
            Bitmap bitmap = null;

            await Task.Run(() =>
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(url);

                bitmap = new Bitmap(stream);

                if (bitmap != null)
                {
                    if (!Directory.Exists(baseFilePath))
                    {
                        Directory.CreateDirectory(baseFilePath);
                    }
                    bitmap.Save(filename, ImageFormat.Png);
                    villager.AvatarUrl = filename;
                }

                stream.Flush();
                stream.Close();
                client.Dispose();
            });

            if (bitmap == null)
            {
                bitmap = CreateDefaultAvatar();
            }

            return bitmap;
        }

        public static Bitmap CreateDefaultAvatar()
        {
            Bitmap bitmap = new Bitmap(128, 128);

            Graphics graphics = Graphics.FromImage(bitmap);

            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            graphics.FillRectangle(System.Drawing.Brushes.DarkGray, rectangle);
            graphics.Dispose();

            return bitmap;
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            BitmapImage bitmapimage = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                if (bitmap == null)
                {
                    bitmap = CreateDefaultAvatar();
                }
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        public static BitmapSource GetImageStream(Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }
    }
}
