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
using s32.Sceh.Classes;
using s32.Sceh.Code;
using s32.Sceh.Data;
using s32.Sceh.WinApp.Code;
using s32.Sceh.WinApp.Translations;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static readonly DependencyProperty SteamProfilesProperty =
            DependencyProperty.Register("SteamProfiles", typeof(List<SteamProfile>), typeof(LoginWindow), new PropertyMetadata(null));

        private static RoutedUICommand _loginCommand;

        static LoginWindow()
        {
            _loginCommand = new RoutedUICommand(Strings.LoginButtonText, "Login", typeof(LoginWindow));
        }

        public LoginWindow()
        {
            InitializeComponent();

            LoadProfiles();

            DataContext = this;
        }

        public static RoutedUICommand LoginCommand
        {
            get { return _loginCommand; }
        }

        public List<SteamProfile> SteamProfiles
        {
            get { return (List<SteamProfile>)GetValue(SteamProfilesProperty); }
            set { SetValue(SteamProfilesProperty, value); }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (cbProfile != null)
                e.CanExecute = cbProfile.SelectedItem != null || !String.IsNullOrWhiteSpace(cbProfile.Text);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string errorMessage;
            var steamProfile = ProfileHelper.GetSteamUser((SteamProfile)cbProfile.SelectedItem, cbProfile.Text, out errorMessage);

            if (errorMessage != null)
            {
                MessageBox.Show(this, errorMessage, Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (steamProfile != null)
            {
                LoadProfiles();
                cbProfile.SelectedItem = steamProfile;
            }
        }

        private void LoadProfiles()
        {
            var profiles = DataManager.GetSteamProfiles();
            var list = new List<SteamProfile>(profiles.Count);
            list.AddRange(profiles);
            list.Sort((a, b) => String.Compare(a.Name, b.Name));
            SteamProfiles = list;
        }
    }
}