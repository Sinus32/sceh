using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using s32.Sceh.Data;

namespace s32.Sceh.WinApp.Code
{
    public class ImageFileToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is ImageFile && typeof(ImageSource).IsAssignableFrom(targetType))
            {
                var filePath = ScehData.LocalFilePath((ImageFile)value);
                if (filePath != null)
                    return new BitmapImage(new Uri(filePath));
                else
                    return null;
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}