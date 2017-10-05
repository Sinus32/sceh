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
using System.Windows.Shapes;
using s32.Sceh.DataStore;
using s32.Sceh.WinApp.Code;
using s32.Sceh.WinApp.Translations;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for InvCompareWindow.xaml
    /// </summary>
    public partial class InvCompareWindow : Window
    {
        public static readonly DependencyProperty OwnerProfileProperty =
            DependencyProperty.Register("OwnerProfile", typeof(SteamProfile), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SecondProfileProperty =
            DependencyProperty.Register("SecondProfile", typeof(SteamProfile), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SteamProfilesProperty =
            DependencyProperty.Register("SteamProfiles", typeof(List<SteamProfile>), typeof(InvCompareWindow), new PropertyMetadata(null));

        private static RoutedUICommand _compareCommand;

        static InvCompareWindow()
        {
            _compareCommand = new RoutedUICommand(Strings.CompareButtonText, "Compare", typeof(LoginWindow));
        }

        public InvCompareWindow()
        {
            InitializeComponent();

            SteamProfiles = ProfileHelper.LoadProfiles();

            DataContext = this;
        }

        public static RoutedUICommand CompareCommand
        {
            get { return _compareCommand; }
        }

        public SteamProfile OwnerProfile
        {
            get { return (SteamProfile)GetValue(OwnerProfileProperty); }
            set { SetValue(OwnerProfileProperty, value); }
        }

        public SteamProfile SecondProfile
        {
            get { return (SteamProfile)GetValue(SecondProfileProperty); }
            set { SetValue(SecondProfileProperty, value); }
        }

        public List<SteamProfile> SteamProfiles
        {
            get { return (List<SteamProfile>)GetValue(SteamProfilesProperty); }
            set { SetValue(SteamProfilesProperty, value); }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (cbOtherProfile != null)
                e.CanExecute = cbOtherProfile.SelectedItem != null || !String.IsNullOrWhiteSpace(cbOtherProfile.Text);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string errorMessage;
            var steamProfile = ProfileHelper.GetSteamUser((SteamProfile)cbOtherProfile.SelectedItem, cbOtherProfile.Text, out errorMessage);

            if (errorMessage != null)
            {
                MessageBox.Show(this, errorMessage, Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (steamProfile != null)
            {
                SteamProfiles = ProfileHelper.LoadProfiles();
                cbOtherProfile.SelectedItem = steamProfile;
                OwnerProfile = steamProfile;
            }
        }
    }
}