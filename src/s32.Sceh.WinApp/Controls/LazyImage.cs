using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using s32.Sceh.Code;
using s32.Sceh.DataStore;
using s32.Sceh.WinApp.Code;

namespace s32.Sceh.WinApp.Controls
{
    public class LazyImage : Image
    {
        public static readonly DependencyProperty ImageFileProperty =
            DependencyProperty.Register("ImageFile", typeof(ImageFile), typeof(LazyImage), new PropertyMetadata(null, ImageFileChanged));

        public static readonly DependencyProperty LazySourceProperty =
            DependencyProperty.Register("LazySource", typeof(ImageSource), typeof(LazyImage), new PropertyMetadata(null));

        public ImageFile ImageFile
        {
            get { return (ImageFile)GetValue(ImageFileProperty); }
            set { SetValue(ImageFileProperty, value); }
        }

        public ImageSource LazySource
        {
            get { return (ImageSource)GetValue(LazySourceProperty); }
            set { SetValue(LazySourceProperty, value); }
        }

        private static void ImageFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var filePath = DataManager.LocalFilePath((ImageFile)e.NewValue);
                if (filePath != null)
                    ImageLoadNotifier.OrderImage(filePath, d);
            }
        }
    }
}