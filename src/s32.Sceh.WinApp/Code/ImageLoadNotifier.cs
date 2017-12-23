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
using s32.Sceh.DataModel;
using s32.Sceh.WinApp.Controls;
using System.Diagnostics;

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
                        var setter = new ImageSourceSetter(imageCtl, imagePath);
                        imageCtl.Dispatcher.Invoke(setter.Action);
                    }
                }
            }
        }

        public static void OrderImage(ImageFile imageFile, LazyImage imageCtl, DownloadPriority newFilePriority, DownloadPriority oldFilePriority)
        {
            var localFilePath = DataManager.LocalFilePath(imageFile);

            if (localFilePath != null && File.Exists(localFilePath))
            {
                var setter = new ImageSourceSetter(imageCtl, localFilePath);
                imageCtl.Dispatcher.Invoke(setter.Action);
                ImageDownloader.EnqueueDownload(imageFile, oldFilePriority, false);
            }
            else
            {
                List<WeakReference<LazyImage>> targets;
                if (_requests.TryGetValue(imageFile, out targets))
                {
                    lock (targets)
                    {
                        for (int i = 0; i < targets.Count; ++i)
                        {
                            LazyImage tmp;
                            if (!targets[i].TryGetTarget(out tmp))
                            {
                                targets[i].SetTarget(imageCtl);
                                return;
                            }
                        }
                        var wr = new WeakReference<LazyImage>(imageCtl);
                        targets.Add(wr);
                    }
                }
                else
                {
                    targets = new List<WeakReference<LazyImage>>();
                    var wr = new WeakReference<LazyImage>(imageCtl);
                    targets.Add(wr);
                    _requests.TryAdd(imageFile, targets);
                }
                ImageDownloader.EnqueueDownload(imageFile, newFilePriority, true);
            }
        }

        private class ImageSourceSetter
        {
            private LazyImage _image;
            private string _localFilePath;

            public ImageSourceSetter(LazyImage image, string localFilePath)
            {
                _image = image;
                _localFilePath = localFilePath;
            }

            public void Action()
            {
                var dc = _image.DataContext;
                string title = "Image";
                
                if (dc is Card)
                    title = ((Card)dc).Name;
                else if (dc is SteamProfile)
                    title = ((SteamProfile)dc).Name;

                //if (IsInView(_image))
                //{
                //    Debug.WriteLine(title, "Image in view");
                    _image.LocalFilePath = _localFilePath;
                //}
                //else
                //{
                //    Debug.WriteLine(title, "Image NOT in view");
                //}
            }

            //private bool IsInView(FrameworkElement reference)
            //{
            //    try
            //    {
            //        var parent = VisualTreeHelper.GetParent(reference);

            //        while (parent != null)
            //        {
            //            if (parent is FrameworkElement)
            //            {
            //                var fe = (FrameworkElement)parent;
            //                if (fe.Visibility != Visibility.Visible)
            //                    return false;

            //                if (fe is ScrollViewer)
            //                {
            //                    var sv = (ScrollViewer)fe;
            //                    GeneralTransform childTransform = reference.TransformToAncestor(sv);
            //                    Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), reference.RenderSize));
            //                    Rect intersection = Rect.Intersect(new Rect(new Point(0, 0), sv.RenderSize), rectangle);
            //                    if (intersection == Rect.Empty)
            //                        return false;
            //                    return IsInView(sv);
            //                }
            //            }

            //            parent = VisualTreeHelper.GetParent(parent) ?? LogicalTreeHelper.GetParent(parent);
            //        }
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine(ex.Message);
            //        return false;
            //    }
            //}
        }
    }
}