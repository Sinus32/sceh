using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using s32.Sceh.Code;
using s32.Sceh.DataStore;
using s32.Sceh.WinApp.Controls;

namespace s32.Sceh.WinApp.Code
{
    public class ImageLoadNotifier
    {
        private static readonly ConcurrentDictionary<ImageFile, List<WeakReference>> _requests;

        static ImageLoadNotifier()
        {
            _requests = new ConcurrentDictionary<ImageFile, List<WeakReference>>();
        }

        public static void FileIsReady(ImageFile imageFile, string imagePath)
        {
            if (String.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return;

            List<WeakReference> targets;
            if (_requests.TryRemove(imageFile, out targets))
            {
                var set = new HashSet<object>();
                foreach (var wr in targets)
                {
                    if (!wr.IsAlive)
                        continue;

                    if (set.Add(wr.Target))
                    {
                        var img = (LazyImage)wr.Target;
                        var setter = new ImageSourceSetter(img, new Uri(imagePath));
                        img.Dispatcher.Invoke(setter.Action);
                    }
                }
            }
        }

        public static void OrderImage(ImageFile imageFile, DependencyObject imageCtl)
        {
            var imagePath = DataManager.LocalFilePath(imageFile);
            if (imagePath != null && File.Exists(imagePath))
            {
                var img = (LazyImage)imageCtl;
                var setter = new ImageSourceSetter(img, new Uri(imagePath));
                img.Dispatcher.Invoke(setter.Action);
            }
            else
            {
                List<WeakReference> targets;
                var wr = new WeakReference(imageCtl);
                if (_requests.TryGetValue(imageFile, out targets))
                {
                    lock (targets)
                        targets.Add(wr);
                }
                else
                {
                    targets = new List<WeakReference>();
                    targets.Add(wr);
                    _requests.TryAdd(imageFile, targets);
                }
            }
        }

        private class ImageSourceSetter
        {
            private LazyImage _image;
            private Uri _uri;

            public ImageSourceSetter(LazyImage image, Uri uri)
            {
                _image = image;
                _uri = uri;
            }

            public void Action()
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.UriSource = _uri;
                bitmapImage.EndInit();
                _image.LazySource = bitmapImage;
            }
        }
    }
}