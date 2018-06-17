using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace s32.Sceh.WinApp.Converters
{
    public class ImageLoader : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!(value is string) || !typeof(ImageSource).Equals(targetType))
                throw new NotSupportedException();

            var localFilePath = (string)value;
            if (String.IsNullOrEmpty(localFilePath))
                return null;

            byte[] byteSource;
            if (TryLoadOrGetFromCache(localFilePath, out byteSource))
                return GetImage(byteSource);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static bool TryLoadOrGetFromCache(string imagePath, out byte[] byteSource)
        {
            const int bufferSize = 256 * 1024;
            byteSource = MemoryCache.Default.Get(imagePath) as byte[];
            if (byteSource != null)
                return true;

            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    using (var ms = new MemoryStream())
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize))
                    {
                        var buffer = new byte[bufferSize];
                        int readed;
                        while ((readed = stream.Read(buffer, 0, bufferSize)) != 0)
                            ms.Write(buffer, 0, readed);
                        ms.Flush();
                        byteSource = ms.ToArray();
                    }
                    var policy = new CacheItemPolicy();
                    policy.SlidingExpiration = new TimeSpan(0, 6, 0);
                    MemoryCache.Default.Set(imagePath, byteSource, policy);
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(250);
                }
            }
            return false;
        }

        private BitmapImage GetImage(byte[] byteSource)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(byteSource);
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
