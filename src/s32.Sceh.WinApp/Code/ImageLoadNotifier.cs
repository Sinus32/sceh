using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using s32.Sceh.WinApp.Controls;

namespace s32.Sceh.WinApp.Code
{
    public class ImageLoadNotifier
    {
        private static readonly ConcurrentDictionary<string, List<WeakReference>> _requests;

        static ImageLoadNotifier()
        {
            _requests = new ConcurrentDictionary<string, List<WeakReference>>();
        }

        public static void FileIsReady(string imagePath)
        {
            if (String.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return;

            List<WeakReference> targets;
            if (_requests.TryRemove(imagePath, out targets))
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

        public static void OrderImage(string imagePath, object image)
        {
            if (File.Exists(imagePath))
            {
                var img = (LazyImage)image;
                var setter = new ImageSourceSetter(img, new Uri(imagePath));
                img.Dispatcher.Invoke(setter.Action);
            }
            else
            {
                List<WeakReference> targets;
                var wr = new WeakReference(image);
                if (_requests.TryGetValue(imagePath, out targets))
                {
                    lock (targets)
                        targets.Add(wr);
                }
                else
                {
                    targets = new List<WeakReference>();
                    targets.Add(wr);
                    _requests.TryAdd(imagePath, targets);
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
                _image.LazySource = new BitmapImage(_uri);
            }
        }
    }
}