using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using s32.Sceh.Code;
using s32.Sceh.DataModel;
using s32.Sceh.WinApp.Code;
using System.Windows.Threading;
using System.Diagnostics;

namespace s32.Sceh.WinApp.Controls
{
    public class LazyImage : Image
    {
        public static readonly DependencyProperty DelayedLocalFilePathProperty =
            DependencyProperty.Register("DelayedLocalFilePath", typeof(string), typeof(LazyImage), new PropertyMetadata(null));

        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(int), typeof(LazyImage), new PropertyMetadata(0));

        public static readonly DependencyProperty ImageFileProperty =
            DependencyProperty.Register("ImageFile", typeof(ImageFile), typeof(LazyImage), new PropertyMetadata(null, ImageFileChanged));

        public static readonly DependencyProperty LocalFilePathProperty =
            DependencyProperty.Register("LocalFilePath", typeof(string), typeof(LazyImage), new PropertyMetadata(null, LocalFilePathChanged));

        public static readonly DependencyProperty NewFilePriorityProperty =
            DependencyProperty.Register("NewFilePriority", typeof(DownloadPriority), typeof(LazyImage), new PropertyMetadata(null));

        public static readonly DependencyProperty OldFilePriorityProperty =
            DependencyProperty.Register("OldFilePriority", typeof(DownloadPriority), typeof(LazyImage), new PropertyMetadata(null));

        private bool _delayFinished;
        private DispatcherTimer _timer;

        public LazyImage()
        {
            this.Loaded += LazyImage_Loaded;
            this.Unloaded += LazyImage_Unloaded;
        }

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public string DelayedLocalFilePath
        {
            get { return (string)GetValue(DelayedLocalFilePathProperty); }
            set { SetValue(DelayedLocalFilePathProperty, value); }
        }

        public ImageFile ImageFile
        {
            get { return (ImageFile)GetValue(ImageFileProperty); }
            set { SetValue(ImageFileProperty, value); }
        }

        public string LocalFilePath
        {
            get { return (string)GetValue(LocalFilePathProperty); }
            set { SetValue(LocalFilePathProperty, value); }
        }

        public DownloadPriority NewFilePriority
        {
            get { return (DownloadPriority)GetValue(NewFilePriorityProperty); }
            set { SetValue(NewFilePriorityProperty, value); }
        }

        public DownloadPriority OldFilePriority
        {
            get { return (DownloadPriority)GetValue(OldFilePriorityProperty); }
            set { SetValue(OldFilePriorityProperty, value); }
        }

        private static void ImageFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                ImageLoadNotifier.OrderImage((ImageFile)e.NewValue, (LazyImage)d, ((LazyImage)d).NewFilePriority, ((LazyImage)d).OldFilePriority);
        }

        private static void LocalFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (LazyImage)d;
            if (self._delayFinished && self.LocalFilePath != null)
                self.DelayedLocalFilePath = self.LocalFilePath;
        }

        private void LazyImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Delay > 0)
            {
                if (_timer == null)
                {
                    _timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher);
                    _timer.Interval = TimeSpan.FromMilliseconds(Delay);
                    _timer.Tick += Timer_Tick;
                }
                _timer.Start();
            }
            else
            {
                _delayFinished = true;
                if (LocalFilePath != null)
                    DelayedLocalFilePath = LocalFilePath;
            }
        }

        private void LazyImage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
                _timer.Stop();
            _delayFinished = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            if (IsLoaded)
            {
                _delayFinished = true;
                if (_delayFinished && LocalFilePath != null)
                    DelayedLocalFilePath = LocalFilePath;
            }
        }
    }
}