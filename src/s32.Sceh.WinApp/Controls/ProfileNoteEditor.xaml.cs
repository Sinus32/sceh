using s32.Sceh.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace s32.Sceh.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for ProfileNoteEditor.xaml
    /// </summary>
    public partial class ProfileNoteEditor : UserControl
    {
        public static readonly DependencyProperty AutoFocusProperty =
            DependencyProperty.Register("AutoFocus", typeof(bool), typeof(ProfileNoteEditor), new PropertyMetadata(null));

        public ProfileNoteEditor()
        {
            InitializeComponent();
        }

        public bool AutoFocus
        {
            get { return (bool)GetValue(AutoFocusProperty); }
            set { SetValue(AutoFocusProperty, value); }
        }

        public List<SteamApp> SteamApps { get; set; }

        private void noteEditForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (AutoFocus)
                Keyboard.Focus(tbEditor);
        }

        #region Commands

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parent = LogicalTreeHelper.GetParent(this);
            if (parent is Panel)
            {
                var panel = (Panel)parent;
                panel.Children.Remove(this);
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //noteEditPopup.IsOpen = false;
        }

        #endregion Commands
    }
}