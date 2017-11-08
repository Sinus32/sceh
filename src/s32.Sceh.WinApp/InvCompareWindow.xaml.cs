using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        private CardsCompareManager _cardsCompareManager;

        private BackgroundWorker _inventoryLoadWorker;

        static InvCompareWindow()
        {
            _compareCommand = new RoutedCommand("Compare", typeof(LoginWindow));
            _showHideCardsCommand = new RoutedCommand("ShowHideCards", typeof(LoginWindow));
            _openMarketPageCommand = new RoutedCommand("OpenMarketPage", typeof(LoginWindow));
            _openStorePageCommand = new RoutedCommand("OpenStorePage", typeof(LoginWindow));
            _openBadgePageCommand = new RoutedCommand("OpenBadgePage", typeof(LoginWindow));
            _openTradingForumCommand = new RoutedCommand("OpenTradingForum", typeof(LoginWindow));
        }

        public InvCompareWindow()
        {
            _cardsCompareManager = new CardsCompareManager();
            _inventoryLoadWorker = new BackgroundWorker();
            _inventoryLoadWorker.DoWork += InventoryLoadWorker_DoWork;
            _inventoryLoadWorker.RunWorkerCompleted += InventoryLoadWorker_RunWorkerCompleted;

            InitializeComponent();

            SteamProfiles = ProfileHelper.LoadProfiles();

            DataContext = this;
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

        private void InventoryLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (InventoryLoadWorkerArgs)e.Argument;
            var ownerInv = DataManager.GetSteamUserInventory(args.OwnerProfile, args.ForceRefresh);
            var secondInv = DataManager.GetSteamUserInventory(args.SecondProfile, args.ForceRefresh);
            _cardsCompareManager.Fill(ownerInv.Cards, secondInv.Cards);
            _cardsCompareManager.ShowHideCards(CardsCompareManager.ShowTradeSugestionsStrategy);
            e.Result = new InventoryLoadWorkerResult() { OwnerInv = ownerInv, SecondInv = secondInv };
        }

        private void InventoryLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        #region Commands

        private static RoutedCommand _compareCommand, _showHideCardsCommand, _openMarketPageCommand, _openStorePageCommand, _openBadgePageCommand, _openTradingForumCommand;

        public static RoutedCommand CompareCommand
        {
            get { return _compareCommand; }
        }

        public static RoutedCommand ShowHideCardsCommand
        {
            get { return _showHideCardsCommand; }
        }

        public static RoutedCommand OpenMarketPageCommand
        {
            get { return _openMarketPageCommand; }
        }

        public static RoutedCommand OpenStorePageCommand
        {
            get { return _openStorePageCommand; }
        }

        public static RoutedCommand OpenBadgePageCommand
        {
            get { return _openBadgePageCommand; }
        }

        public static RoutedCommand OpenTradingForumCommand
        {
            get { return _openTradingForumCommand; }
        }

        private void OpenMarketPageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is Card;
        }

        private void OpenMarketPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "http://steamcommunity.com/market/listings/{0}/{1}";
            string url = null;
            if (e.Parameter is Card)
            {
                var card = (Card)e.Parameter;
                url = String.Format(PATTERN, card.AppId, card.MarketHashName);
            }

            if (url != null)
                System.Diagnostics.Process.Start(url);
        }

        private void OpenStorePageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamApp || e.Parameter is Card;
        }

        private void OpenStorePageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "http://store.steampowered.com/app/{0}/";
            string url = null;
            if (e.Parameter is SteamApp)
            {
                var steamApp = (SteamApp)e.Parameter;
                url = String.Format(PATTERN, steamApp.Id);
            }
            else if (e.Parameter is Card)
            {
                var card = (Card)e.Parameter;
                url = String.Format(PATTERN, card.MarketFeeApp);
            }

            if (url != null)
                System.Diagnostics.Process.Start(url);
        }

        private void OpenBadgePageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is Card || e.Parameter is SteamApp;
        }

        private void OpenBadgePageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "{0}/gamecards/{1}/";
            var myInvLink = SteamDataDownloader.GetProfileUri(OwnerProfile, SteamUrlPattern.CommunityPage);
            string url = null;
            if (e.Parameter is SteamApp)
            {
                var steamApp = (SteamApp)e.Parameter;
                url = String.Format(PATTERN, myInvLink, steamApp.Id);
            }
            else if (e.Parameter is Card)
            {
                var card = (Card)e.Parameter;
                url = String.Format(PATTERN, myInvLink, card.MarketFeeApp);
            }

            if (url != null)
                System.Diagnostics.Process.Start(url);
        }

        private void OpenTradingForumCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is Card || e.Parameter is SteamApp;
        }

        private void OpenTradingForumCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "http://steamcommunity.com/app/{0}/tradingforum/";
            string url = null;
            if (e.Parameter is SteamApp)
            {
                var steamApp = (SteamApp)e.Parameter;
                url = String.Format(PATTERN, steamApp.Id);
            }
            else if (e.Parameter is Card)
            {
                var card = (Card)e.Parameter;
                url = String.Format(PATTERN, card.MarketFeeApp);
            }

            if (url != null)
                System.Diagnostics.Process.Start(url);
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

        private void ShowHideCardsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is CardsCompareManager.ShowHideStrategy;
        }

        private void ShowHideCardsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var steamApps = SteamApps;
            if (steamApps != null && steamApps.Count > 0)
            {
                _cardsCompareManager.ShowHideCards((CardsCompareManager.ShowHideStrategy)e.Parameter);

                var cvs = (CollectionViewSource)this.FindResource("steamAppsView");
                cvs.View.Refresh();
            }
        }

        #endregion

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

        private void CollectionViewSource_FilterByHideProp(object sender, FilterEventArgs e)
        {
            var steamApp = e.Item as SteamApp;
            if (steamApp != null)
            {
                e.Accepted = !steamApp.Hide;
                return;
            }

            var card = e.Item as Card;
            if (card != null)
            {
                e.Accepted = !card.Hide;
                return;
            }

            e.Accepted = true;
        }
    }
}