using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using s32.Sceh.Code;
using s32.Sceh.DataModel;
using s32.Sceh.WinApp.Helpers;
using s32.Sceh.WinApp.Translations;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static readonly DependencyProperty AutoLogInProperty =
            DependencyProperty.Register("AutoLogIn", typeof(bool), typeof(LoginWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty SteamProfilesProperty =
            DependencyProperty.Register("SteamProfiles", typeof(List<SteamProfile>), typeof(LoginWindow), new PropertyMetadata(null));

        public LoginWindow()
        {
            InitializeComponent();

            SteamProfiles = ProfileHelper.LoadProfiles(null);
            AutoLogIn = DataManager.AutoLogIn;

            DataContext = this;

            var profile = DataManager.LastSteamProfile;
            if (profile != null)
            {
                cbProfile.SelectedItem = profile;
            }
        }

        public bool AutoLogIn
        {
            get { return (bool)GetValue(AutoLogInProperty); }
            set { SetValue(AutoLogInProperty, value); }
        }

        public List<SteamProfile> SteamProfiles
        {
            get { return (List<SteamProfile>)GetValue(SteamProfilesProperty); }
            set { SetValue(SteamProfilesProperty, value); }
        }

        private void LoginCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (cbProfile != null)
                e.CanExecute = cbProfile.SelectedItem != null || !String.IsNullOrWhiteSpace(cbProfile.Text);
        }

        private void LoginCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string errorMessage;
            var steamProfile = ProfileHelper.GetSteamUser((SteamProfile)cbProfile.SelectedItem, cbProfile.Text, out errorMessage);

            if (errorMessage != null)
            {
                MessageBox.Show(this, errorMessage, Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (steamProfile != null)
            {
                DataManager.LastSteamProfile = steamProfile;
                DataManager.AutoLogIn = AutoLogIn;

                var cmpWindow = new InvCompareWindow();
                cmpWindow.OwnerProfile = steamProfile;
                cmpWindow.Show();
                this.Close();
            }
        }
    }
}
