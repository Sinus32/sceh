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
using s32.Sceh.DataModel;

namespace s32.Sceh.WinApp.Controls
{
    public class UrlToImageFileConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty DirectoryProperty =
            DependencyProperty.Register("Directory", typeof(ImageDirectory), typeof(UrlToImageFileConverter), new PropertyMetadata(null));

        public ImageDirectory Directory
        {
            get { return (ImageDirectory)GetValue(DirectoryProperty); }
            set { SetValue(DirectoryProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!typeof(ImageFile).Equals(targetType))
                throw new NotSupportedException();

            if (Directory == null)
                return DependencyProperty.UnsetValue;

            ImageFile result;
            if (value is Card)
                result = DataManager.GetOrCreateImageFile((Card)value, Directory);
            else if (value is Uri)
                result = DataManager.GetOrCreateImageFile((Uri)value, Directory);
            else
                throw new NotSupportedException();

            return result ?? DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var imageFile = value as ImageFile;
            if (imageFile == null || !typeof(Uri).Equals(targetType))
                throw new NotSupportedException();

            return imageFile.ImageUrl;
        }
    }
}