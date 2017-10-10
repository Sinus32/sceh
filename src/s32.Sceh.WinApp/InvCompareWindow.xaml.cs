using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using s32.Sceh.Code;
using s32.Sceh.DataModel;
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
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty OwnerInvErrorProperty =
            DependencyProperty.Register("OwnerInvError", typeof(string), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty OwnerProfileProperty =
            DependencyProperty.Register("OwnerProfile", typeof(SteamProfile), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SecondInvErrorProperty =
            DependencyProperty.Register("SecondInvError", typeof(string), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SecondProfileProperty =
            DependencyProperty.Register("SecondProfile", typeof(SteamProfile), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SteamAppsProperty =
            DependencyProperty.Register("SteamApps", typeof(List<SteamApp>), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SteamProfilesProperty =
            DependencyProperty.Register("SteamProfiles", typeof(List<SteamProfile>), typeof(InvCompareWindow), new PropertyMetadata(null));

        private static RoutedCommand _compareCommand, _showHideCards;

        private CardsCompareManager _cardsCompareManager;

        private BackgroundWorker _inventoryLoadWorker;

        static InvCompareWindow()
        {
            _compareCommand = new RoutedCommand("Compare", typeof(LoginWindow));
            _showHideCards = new RoutedCommand("ShowHideCards", typeof(LoginWindow));
        }

        public InvCompareWindow()
        {
            _cardsCompareManager = new CardsCompareManager();
            _inventoryLoadWorker = new BackgroundWorker();
            _inventoryLoadWorker.DoWork += _inventoryLoadWorker_DoWork;
            _inventoryLoadWorker.RunWorkerCompleted += _inventoryLoadWorker_RunWorkerCompleted;

            InitializeComponent();

            SteamProfiles = ProfileHelper.LoadProfiles();

            DataContext = this;
        }

        public static RoutedCommand CompareCommand
        {
            get { return _compareCommand; }
        }

        public static RoutedCommand ShowHideCards
        {
            get { return _showHideCards; }
        }

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        public string OwnerInvError
        {
            get { return (string)GetValue(OwnerInvErrorProperty); }
            set { SetValue(OwnerInvErrorProperty, value); }
        }

        public SteamProfile OwnerProfile
        {
            get { return (SteamProfile)GetValue(OwnerProfileProperty); }
            set { SetValue(OwnerProfileProperty, value); }
        }

        public string SecondInvError
        {
            get { return (string)GetValue(SecondInvErrorProperty); }
            set { SetValue(SecondInvErrorProperty, value); }
        }

        public SteamProfile SecondProfile
        {
            get { return (SteamProfile)GetValue(SecondProfileProperty); }
            set { SetValue(SecondProfileProperty, value); }
        }

        public List<SteamApp> SteamApps
        {
            get { return (List<SteamApp>)GetValue(SteamAppsProperty); }
            set { SetValue(SteamAppsProperty, value); }
        }

        public List<SteamProfile> SteamProfiles
        {
            get { return (List<SteamProfile>)GetValue(SteamProfilesProperty); }
            set { SetValue(SteamProfilesProperty, value); }
        }

        private void _inventoryLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (InventoryLoadWorkerArgs)e.Argument;
            var ownerInv = DataManager.GetSteamUserInventory(args.OwnerProfile, args.ForceRefresh);
            var secondInv = DataManager.GetSteamUserInventory(args.SecondProfile, args.ForceRefresh);
            _cardsCompareManager.Fill(ownerInv.Cards, secondInv.Cards);
            _cardsCompareManager.ShowHideCards(CardsCompareManager.ShowTradeSugestionsStrategy);
            e.Result = new InventoryLoadWorkerResult() { OwnerInv = ownerInv, SecondInv = secondInv };
        }

        private void _inventoryLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (InventoryLoadWorkerResult)e.Result;
            if (result != null)
            {
                OwnerInvError = result.OwnerInv.ErrorMessage;
                SecondInvError = result.SecondInv.ErrorMessage;
                MakeErrorMessage();
                var steamApps = new List<SteamApp>(_cardsCompareManager.SteamApps.Count);
                steamApps.AddRange(_cardsCompareManager.SteamApps);
                SteamApps = steamApps;
            }
        }

        private void CompareCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_inventoryLoadWorker == null || _inventoryLoadWorker.IsBusy)
                e.CanExecute = false;
            else if (cbOtherProfile != null)
                e.CanExecute = cbOtherProfile.SelectedItem != null || !String.IsNullOrWhiteSpace(cbOtherProfile.Text);
        }

        private void CompareCommand_Executed(object sender, ExecutedRoutedEventArgs e)
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
                SecondProfile = steamProfile;

                var args = new InventoryLoadWorkerArgs()
                {
                    OwnerProfile = OwnerProfile,
                    SecondProfile = SecondProfile,
                    ForceRefresh = false
                };

                _inventoryLoadWorker.RunWorkerAsync(args);
            }
        }

        private void MakeErrorMessage()
        {
            if (OwnerInvError == null && SecondInvError == null)
            {
                ErrorMessage = null;
                return;
            }

            var sb = new StringBuilder();
            if (OwnerInvError != null)
                sb.AppendFormat(Strings.OwnerInvErrorMessage, OwnerInvError).AppendLine();
            if (SecondInvError != null)
                sb.AppendFormat(Strings.SecondInvErrorMessage, SecondProfile.Name, SecondInvError).AppendLine();
            ErrorMessage = sb.ToString();
        }

        private void ShowHideCards_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is CardsCompareManager.ShowHideStrategy;
        }

        private void ShowHideCards_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _cardsCompareManager.ShowHideCards((CardsCompareManager.ShowHideStrategy)e.Parameter);
        }

        private class InventoryLoadWorkerArgs
        {
            public bool ForceRefresh;
            public SteamProfile OwnerProfile;
            public SteamProfile SecondProfile;
        }

        private class InventoryLoadWorkerResult
        {
            public UserInventory OwnerInv;
            public UserInventory SecondInv;
        }
    }
}