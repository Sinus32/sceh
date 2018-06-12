using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace s32.Sceh.WinApp.Controls
{
    public class PopupButton : ToggleButton
    {
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Popup", typeof(Popup), typeof(PopupButton), new UIPropertyMetadata(null, OnPopupChanged));

        public PopupButton()
        {
            Binding binding = new Binding("Popup.IsOpen");
            binding.Source = this;
            this.SetBinding(IsCheckedProperty, binding);
            DataContextChanged += PopupButton_DataContextChanged;
        }

        public Popup Popup
        {
            get { return (Popup)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        protected override void OnClick()
        {
            if (Popup != null)
            {
                Popup.PlacementTarget = this;
                Popup.IsOpen = true;
            }
        }

        private static void OnPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popupButton = (PopupButton)d;
            var popup = (Popup)e.NewValue;
            popup.DataContext = popupButton.DataContext;
            popup.StaysOpen = false;
        }

        private void PopupButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Popup != null)
                Popup.DataContext = DataContext;
        }
    }
}
