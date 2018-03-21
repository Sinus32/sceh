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
    public class DropDownButton : ToggleButton
    {
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu", typeof(ContextMenu), typeof(DropDownButton), new UIPropertyMetadata(null, OnMenuChanged));

        public DropDownButton()
        {
            Binding binding = new Binding("Menu.IsOpen");
            binding.Source = this;
            this.SetBinding(IsCheckedProperty, binding);
            DataContextChanged += DropDownButton_DataContextChanged;
        }

        public ContextMenu Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        protected override void OnClick()
        {
            if (Menu != null)
            {
                Menu.PlacementTarget = this;
                Menu.Placement = PlacementMode.Bottom;
                Menu.IsOpen = true;
            }
        }

        private static void OnMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dropDownButton = (DropDownButton)d;
            var contextMenu = (ContextMenu)e.NewValue;
            contextMenu.DataContext = dropDownButton.DataContext;
        }

        private void DropDownButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Menu != null)
                Menu.DataContext = DataContext;
        }
    }
}