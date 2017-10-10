using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace s32.Sceh.WinApp.Controls
{
    public class BooleanToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ValueForFalseProperty =
            DependencyProperty.Register("ValueForFalse", typeof(Visibility), typeof(BooleanToVisibilityConverter), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty ValueForTrueProperty =
            DependencyProperty.Register("ValueForTrue", typeof(Visibility), typeof(BooleanToVisibilityConverter), new PropertyMetadata(Visibility.Visible));

        public Visibility ValueForFalse
        {
            get { return (Visibility)GetValue(ValueForFalseProperty); }
            set { SetValue(ValueForFalseProperty, value); }
        }

        public Visibility ValueForTrue
        {
            get { return (Visibility)GetValue(ValueForTrueProperty); }
            set { SetValue(ValueForTrueProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            if (!(value is bool) || !typeof(Visibility).Equals(targetType))
                throw new NotSupportedException();

            return ((bool)value) ? ValueForTrue : ValueForFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            if (!(value is Visibility) || !typeof(bool).Equals(targetType))
                throw new NotSupportedException();

            var val = (Visibility)value;
            if (val == ValueForTrue)
                return true;
            if (val == ValueForFalse)
                return false;
            return DependencyProperty.UnsetValue;
        }
    }
}