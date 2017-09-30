using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using s32.Sceh.Data;
using s32.Sceh.WinApp.Code;

namespace s32.Sceh.WinApp.Controls
{
    public class LazyImage : Image
    {
        public static readonly DependencyProperty ImageFileProperty =
            DependencyProperty.Register("ImageFile", typeof(ImageFile), typeof(LazyImage), new PropertyMetadata(null, ImageFileChanged));

        private static void ImageFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                ((Image)d).Source = null;
            }
            else
            {
                var filePath = ScehData.LocalFilePath((ImageFile)e.NewValue);
                if (filePath != null)
                    ImageLoadNotifier.OrderImage(filePath, d);
            }
        }

        public ImageFile ImageFile
        {
            get { return (ImageFile)GetValue(ImageFileProperty); }
            set { SetValue(ImageFileProperty, value); }
        }
    }
}