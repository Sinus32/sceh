using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using s32.Sceh.Data;
using s32.Sceh.WinApp.Translations;

namespace s32.Sceh.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for SteamProfileComboBox.xaml
    /// </summary>
    public partial class SteamProfileComboBox : UserControl
    {
        public static readonly DependencyProperty SelectedProfileProperty =
            DependencyProperty.Register("SelectedProfile", typeof(SteamProfile), typeof(SteamProfileComboBox), new PropertyMetadata(null));

        public static readonly DependencyProperty SteamProfilesProperty =
            DependencyProperty.Register("SteamProfiles", typeof(List<SteamProfile>), typeof(SteamProfileComboBox), new PropertyMetadata(new List<SteamProfile>()));

        private static RoutedUICommand _checkCommand;

        static SteamProfileComboBox()
        {
            _checkCommand = new RoutedUICommand(Strings.CheckButtonCommand, "Check", typeof(SteamProfileComboBox));
        }

        public SteamProfileComboBox()
        {
            InitializeComponent();

            LoadProfiles();

            DataContext = this;
        }

        public static RoutedUICommand CheckCommand
        {
            get { return _checkCommand; }
        }

        public ComboBox ProfileComboBox
        {
            get { return cbName; }
        }

        public SteamProfile SelectedProfile
        {
            get { return (SteamProfile)GetValue(SelectedProfileProperty); }
            set { SetValue(SelectedProfileProperty, value); }
        }

        public List<SteamProfile> SteamProfiles
        {
            get { return (List<SteamProfile>)GetValue(SteamProfilesProperty); }
            set { SetValue(SteamProfilesProperty, value); }
        }

        private void Check_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (cbName != null)
                e.CanExecute = cbName.SelectedItem != null || !String.IsNullOrWhiteSpace(cbName.Text);
        }

        private void Check_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void LoadProfiles()
        {
            if (ScehData.DataFile == null || ScehData.DataFile.SteamProfiles == null)
            {
                SteamProfiles = new List<SteamProfile>();
                return;
            }

            var list = new List<SteamProfile>(ScehData.DataFile.SteamProfiles);
            list.Sort((a, b) => String.Compare(a.Name, b.Name));
            SteamProfiles = list;
        }
    }
}