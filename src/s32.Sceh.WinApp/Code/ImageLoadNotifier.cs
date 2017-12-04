using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using s32.Sceh.Code;
using s32.Sceh.DataStore;
using s32.Sceh.WinApp.Controls;

namespace s32.Sceh.WinApp.Code
{
    public class ImageLoadNotifier
    {
        private static readonly ConcurrentDictionary<ImageFile, List<WeakReference<LazyImage>>> _requests;

        static ImageLoadNotifier()
        {
            _requests = new ConcurrentDictionary<ImageFile, List<WeakReference<LazyImage>>>();
        }

        public static void FileIsReady(ImageFile imageFile, string imagePath)
        {
            if (String.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return;

            List<WeakReference<LazyImage>> targets;
            if (_requests.TryRemove(imageFile, out targets))
            {
                var set = new HashSet<object>();
                foreach (var wr in targets)
                {
                    LazyImage imageCtl;
                    if (wr.TryGetTarget(out imageCtl) && set.Add(imageCtl))
                    {
                        var image = TryLoadOrGetFromCache(imagePath);
                        if (image != null)
                        {
                            var setter = new ImageSourceSetter(imageCtl, image);
                            imageCtl.Dispatcher.Invoke(setter.Action);
                        }
                    }
                }
            }
        }

        public static void OrderImage(ImageFile imageFile, LazyImage imageCtl)
        {
            var imagePath = DataManager.LocalFilePath(imageFile);
            if (imagePath != null && File.Exists(imagePath))
            {
                var image = TryLoadOrGetFromCache(imagePath);
                if (image != null)
                {
                    var setter = new ImageSourceSetter(imageCtl, image);
                    imageCtl.Dispatcher.Invoke(setter.Action);
                }
            }
            else
            {
                List<WeakReference<LazyImage>> targets;
                var wr = new WeakReference<LazyImage>(imageCtl);
                if (_requests.TryGetValue(imageFile, out targets))
                {
                    lock (targets)
                        targets.Add(wr);
                }
                else
                {
                    targets = new List<WeakReference<LazyImage>>();
                    targets.Add(wr);
                    _requests.TryAdd(imageFile, targets);
                }
            }
        }

        private static byte[] TryLoadOrGetFromCache(string imagePath)
        {
            const int bufferSize = 256 * 1024;
            var result = MemoryCache.Default.Get(imagePath) as byte[];
            if (result == null)
            {
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
                            result = ms.ToArray();
                        }
                        var policy = new CacheItemPolicy();
                        policy.SlidingExpiration = new TimeSpan(0, 6, 0);
                        MemoryCache.Default.Set(imagePath, result, policy);
                        return result;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(250);
                    }
                }
            }
            return result;
        }

        private class ImageSourceSetter
        {
            private LazyImage _image;
            private byte[] _source;

            public ImageSourceSetter(LazyImage image, byte[] source)
            {
                _image = image;
                _source = source;
            }

            public void Action()
            {
                var bitmapImage = GetImage();
                if (bitmapImage != null)
                {
                    _image.LazySource = bitmapImage;
                    _image.IsReady = true;
                }
                return;
            }

            private BitmapImage GetImage()
            {
                try
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(_source);
                    //double height = _image.Height;
                    //if (height >= 1.0)
                    //    bitmapImage.DecodePixelHeight = (int)height;
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
}