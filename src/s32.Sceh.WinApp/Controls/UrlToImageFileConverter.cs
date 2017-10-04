using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using s32.Sceh.Code;
using s32.Sceh.DataStore;

namespace s32.Sceh.WinApp.Controls
{
    public class UrlToImageFileConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty DirectoryProperty =
            DependencyProperty.Register("Directory", typeof(ImageDirectory), typeof(UrlToImageFileConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty NewFilePriorityProperty =
            DependencyProperty.Register("NewFilePriority", typeof(DownloadPriority), typeof(UrlToImageFileConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty OldFilePriorityProperty =
            DependencyProperty.Register("OldFilePriority", typeof(DownloadPriority), typeof(UrlToImageFileConverter), new PropertyMetadata(null));

        public ImageDirectory Directory
        {
            get { return (ImageDirectory)GetValue(DirectoryProperty); }
            set { SetValue(DirectoryProperty, value); }
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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var imageUrl = value as string;
            if (String.IsNullOrEmpty(imageUrl) || !typeof(ImageFile).Equals(targetType))
                throw new NotSupportedException();

            if (Directory == null)
                return null;

            bool isNew;
            ImageFile result = DataManager.GetOrCreateImageFile(imageUrl, Directory, out isNew);

            var filePath = DataManager.LocalFilePath(result);
            var forceDownload = filePath == null || !File.Exists(filePath);
            var priority = isNew || forceDownload ? NewFilePriority : OldFilePriority;
            if (priority != null)
                ImageDownloader.EnqueueDownload(result, priority, forceDownload);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var imageFile = value as ImageFile;
            if (imageFile == null || !typeof(String).Equals(targetType))
                throw new NotSupportedException();

            return imageFile.ImageUrl;
        }
    }
}