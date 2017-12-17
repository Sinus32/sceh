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

namespace s32.Sceh.WinApp.Controls
{
    public class LazyImage : Image
    {
        public static readonly DependencyProperty ImageFileProperty =
            DependencyProperty.Register("ImageFile", typeof(ImageFile), typeof(LazyImage), new PropertyMetadata(null, ImageFileChanged));

        public static readonly DependencyProperty LocalFilePathProperty =
            DependencyProperty.Register("LocalFilePath", typeof(string), typeof(LazyImage), new PropertyMetadata(null));

        public static readonly DependencyProperty NewFilePriorityProperty =
            DependencyProperty.Register("NewFilePriority", typeof(DownloadPriority), typeof(LazyImage), new PropertyMetadata(null));

        public static readonly DependencyProperty OldFilePriorityProperty =
            DependencyProperty.Register("OldFilePriority", typeof(DownloadPriority), typeof(LazyImage), new PropertyMetadata(null));

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
    }
}