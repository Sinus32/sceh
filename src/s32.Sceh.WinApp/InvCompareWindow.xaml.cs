using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
            try
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
            catch (Exception ex)
            {
                var sb = new StringBuilder(ex.Message);
                for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
                    sb.AppendLine().Append(inner.Message);
                MessageBox.Show(sb.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Debugger.IsAttached)
                    Debugger.Break();
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

        private void CardButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            const string PATTERN = "http://steamcommunity.com/market/listings/{0}/{1}";
            var card = ((FrameworkElement)sender).DataContext as Card;
            if (card != null)
            {
                var url = String.Format(PATTERN, card.AppId, card.MarketHashName);
                if (url != null)
                    System.Diagnostics.Process.Start(url);
            }
        }

        private void ChangeProfileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !SteamDataDownloader.Info.IsInProgress;
        }

        private void ChangeProfileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void EditNoteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfile;
        }

        private void EditNoteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void CopyNameCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamApp || e.Parameter is Card;
        }

        private void CopyNameCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamApp)
            {
                var steamApp = (SteamApp)e.Parameter;
                Clipboard.SetText(steamApp.Name);
            }
            else if (e.Parameter is Card)
            {
                var card = (Card)e.Parameter;
                Clipboard.SetText(card.Name);
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
                steamProfile.LastUse = DateTime.UtcNow;
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

        private void ExitAppCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitAppCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenBadgePageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is Card || e.Parameter is SteamApp;
        }

        private void OpenBadgePageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "{0}/gamecards/{1}/";
            string url = null;
            if (e.Parameter is SteamApp)
            {
                var steamApp = (SteamApp)e.Parameter;
                var invLink = SteamDataDownloader.GetProfileUri(OwnerProfile, SteamUrlPattern.CommunityPage);
                url = String.Format(PATTERN, invLink, steamApp.Id);
            }
            else if (e.Parameter is Card)
            {
                var card = (Card)e.Parameter;
                var invLink = SteamDataDownloader.GetProfileUri(card.Owner, SteamUrlPattern.CommunityPage);
                url = String.Format(PATTERN, invLink, card.MarketFeeApp);
            }

            if (url != null)
                System.Diagnostics.Process.Start(url);
        }

        private void OpenInventoryPageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenInventoryPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.Inventory);
                System.Diagnostics.Process.Start(url.ToString());
            }
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

        private void OpenProfilePageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenProfilePageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.CommunityPage);
                System.Diagnostics.Process.Start(url.ToString());
            }
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

        private void OpenUserBadgesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenUserBadgesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.Badges);
                System.Diagnostics.Process.Start(url.ToString());
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

        #endregion Commands

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