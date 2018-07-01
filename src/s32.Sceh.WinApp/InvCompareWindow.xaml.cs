using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using s32.Sceh.Code;
using s32.Sceh.DataModel;
using s32.Sceh.WinApp.Controls;
using s32.Sceh.WinApp.Helpers;
using s32.Sceh.WinApp.Translations;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for InvCompareWindow.xaml
    /// </summary>
    public partial class InvCompareWindow : Window
    {
        public static readonly DependencyProperty CompareWithSelfProperty =
            DependencyProperty.Register("CompareWithSelf", typeof(bool), typeof(InvCompareWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty MyProfileSelectedProperty =
            DependencyProperty.Register("MyProfileSelected", typeof(bool), typeof(InvCompareWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty OwnerProfileProperty =
            DependencyProperty.Register("OwnerProfile", typeof(SteamProfile), typeof(InvCompareWindow), new PropertyMetadata(null, OwnerProfileChange));

        public static readonly DependencyProperty SecondProfileProperty =
            DependencyProperty.Register("SecondProfile", typeof(SteamProfile), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SteamAppsProperty =
            DependencyProperty.Register("SteamApps", typeof(List<SteamApp>), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SteamProfilesProperty =
            DependencyProperty.Register("SteamProfiles", typeof(List<SteamProfile>), typeof(InvCompareWindow), new PropertyMetadata(null));

        private readonly CardsCompareManager _cardsCompareManager;
        private readonly BackgroundWorker _inventoryLoadWorker;

        public InvCompareWindow()
        {
            _cardsCompareManager = new CardsCompareManager();
            _inventoryLoadWorker = new BackgroundWorker();
            _inventoryLoadWorker.DoWork += InventoryLoadWorker_DoWork;
            _inventoryLoadWorker.RunWorkerCompleted += InventoryLoadWorker_RunWorkerCompleted;

            InitializeComponent();

            DataContext = this;

            var binding = new Binding("SelectedItem");
            binding.Source = cbOtherProfile;
            binding.Converter = new CompareConverter(() => OwnerProfile);
            binding.Mode = BindingMode.OneWay;
            SetBinding(MyProfileSelectedProperty, binding);
        }

        public bool CompareWithSelf
        {
            get { return (bool)GetValue(CompareWithSelfProperty); }
            set { SetValue(CompareWithSelfProperty, value); }
        }

        public bool MyProfileSelected
        {
            get { return (bool)GetValue(MyProfileSelectedProperty); }
            set { SetValue(MyProfileSelectedProperty, value); }
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

        private static void OwnerProfileChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var firstProfileId = e.NewValue == null ? (long?)null : ((SteamProfile)e.NewValue).SteamId;
            ((InvCompareWindow)d).SteamProfiles = ProfileHelper.LoadProfiles(firstProfileId);
        }

        private void ShowException(Exception ex)
        {
            var sb = new StringBuilder(ex.Message);
            for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
                sb.AppendLine().Append(inner.Message);
            MessageBox.Show(sb.ToString(), Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        #region Events

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

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                ((ScrollViewer)sender).LineUp();
            else if (e.Delta < 0)
                ((ScrollViewer)sender).LineDown();
            e.Handled = true;
        }

        #endregion Events

        #region Cards enumerators

        public IEnumerable<Card> AllSelectedCards
        {
            get
            {
                List<SteamApp> steamApps = SteamApps;
                if (steamApps == null)
                    yield break;

                foreach (var app in steamApps)
                {
                    if (app.MyIsSelected)
                        foreach (var card in app.MyCards)
                            if (card.IsSelected)
                                yield return card;

                    if (app.OtherIsSelected)
                        foreach (var card in app.OtherCards)
                            if (card.IsSelected)
                                yield return card;
                }
            }
        }

        public IEnumerable<Card> MySelectedCards
        {
            get
            {
                List<SteamApp> steamApps = SteamApps;
                if (steamApps == null)
                    yield break;

                foreach (var app in steamApps)
                {
                    if (app.MyIsSelected)
                        foreach (var card in app.MyCards)
                            if (card.IsSelected)
                                yield return card;
                }
            }
        }

        public IEnumerable<Card> OtherSelectedCards
        {
            get
            {
                List<SteamApp> steamApps = SteamApps;
                if (steamApps == null)
                    yield break;

                foreach (var app in steamApps)
                {
                    if (app.OtherIsSelected)
                        foreach (var card in app.OtherCards)
                            if (card.IsSelected)
                                yield return card;
                }
            }
        }

        #endregion Cards enumerators

        #region Commands

        private void ChangeProfile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !CommunicationState.Instance.IsInProgress;
        }

        private void ChangeProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Compare_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_inventoryLoadWorker == null || _inventoryLoadWorker.IsBusy)
                e.CanExecute = false;
            else if (cbOtherProfile != null)
                e.CanExecute = cbOtherProfile.SelectedItem != null || !String.IsNullOrWhiteSpace(cbOtherProfile.Text);
        }

        private void Compare_Executed(object sender, ExecutedRoutedEventArgs e)
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
                var firstProfileId = OwnerProfile == null ? (long?)null : OwnerProfile.SteamId;
                SteamProfiles = ProfileHelper.LoadProfiles(firstProfileId);
                cbOtherProfile.SelectedItem = steamProfile;
                SecondProfile = steamProfile;

                RunInventoryLoadWorker(OwnerProfile, SecondProfile, false);
            }
        }

        private void CopyName_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamApp || e.Parameter is Card || e.Parameter is SteamProfile;
        }

        private void CopyName_Executed(object sender, ExecutedRoutedEventArgs e)
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
            else if (e.Parameter is SteamProfile)
            {
                var profile = (SteamProfile)e.Parameter;
                Clipboard.SetText(profile.Name);
            }
        }

        private void CopyNamePlus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is Card;
        }

        private void CopyNamePlus_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is Card)
            {
                var card = (Card)e.Parameter;
                var text = String.Concat(card.MarketFeeAppName, ' ', card.Name);
                Clipboard.SetText(text);
            }
        }

        private void DeselectCards_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var steamApps = SteamApps;
            if (steamApps != null && steamApps.Count > 0 && e.Parameter is IEnumerable<Card>)
            {
                e.CanExecute = ((IEnumerable<Card>)e.Parameter).Any();
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void DeselectCards_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (var card in (IEnumerable<Card>)e.Parameter)
                card.IsSelected = false;
        }

        private void EditNote_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is UserNotes;
        }

        private void EditNote_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var editor = new ProfileNoteEditor();
            Grid.SetRowSpan(editor, mainGrid.RowDefinitions.Count);
            Grid.SetColumnSpan(editor, mainGrid.ColumnDefinitions.Count);
            mainGrid.Children.Add(editor);
            editor.SteamApps = SteamApps;
            editor.Source = (UserNotes)e.Parameter;
            editor.AutoFocus = true;
        }

        private string EncodeMarketHashNameForUrl(string marketHashName)
        {
            var sb = new StringBuilder(marketHashName.Length);
            foreach (char ch in marketHashName)
            {
                if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch >= (char)0x80)
                {
                    sb.Append(ch);
                    continue;
                }

                switch (ch)
                {
                    case '-':
                    case '_':
                    case '.':
                    case '!':
                    case '*':
                    case '(':
                    case ')':
                        sb.Append(ch);
                        break;

                    default:
                        var c = ((int)ch).ToString("X2");
                        sb.Append('%').Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        private void ExitApp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitApp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MakeOffer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is UserNotes && ((UserNotes)e.Parameter).TradeUrl != null;
        }

        private void MakeOffer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var tradeUrl = ((UserNotes)e.Parameter).TradeUrl;
            if (!tradeUrl.IsAbsoluteUri || tradeUrl.Scheme != "https")
            {
                MessageBox.Show(Strings.TradeUrlSeemsToBeInvalid, Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                var escaped = tradeUrl.ToString().Replace(" ", "%20");
                System.Diagnostics.Process.Start(escaped);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void OpenAllMarketPages_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var steamApps = SteamApps;
            if (steamApps != null && steamApps.Count > 0 && e.Parameter is IEnumerable<Card>)
            {
                e.CanExecute = ((IEnumerable<Card>)e.Parameter).Count() < 20;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void OpenAllMarketPages_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "https://steamcommunity.com/market/listings/{0}/{1}";

            var steamApps = SteamApps;
            var cards = e.Parameter as IEnumerable<Card>;
            if (steamApps != null && steamApps.Count > 0 && cards != null)
            {
                foreach (Card card in cards)
                {
                    try
                    {
                        var encoded = EncodeMarketHashNameForUrl(card.MarketHashName);
                        var url = String.Format(PATTERN, card.AppId, encoded);
                        System.Diagnostics.Process.Start(url);
                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex);
                    }
                }
            }
        }

        private void OpenBadgePage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is Card || e.Parameter is SteamApp;
        }

        private void OpenBadgePage_Executed(object sender, ExecutedRoutedEventArgs e)
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

        private void OpenIncomingOffers_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenIncomingOffers_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.IncomingOffers);
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void OpenInventoryPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenInventoryPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.Inventory);
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void OpenMarketPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamApp || e.Parameter is Card;
        }

        private void OpenMarketPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN_STEAM_APP = "https://steamcommunity.com/market/search?appid=753&category_753_Game%5B%5D=tag_app_{0}&category_753_cardborder%5B%5D=tag_cardborder_0&category_753_item_class%5B%5D=tag_item_class_2";
            const string PATTERN_CARD = "https://steamcommunity.com/market/listings/{0}/{1}";
            string url = null;
            if (e.Parameter is SteamApp)
            {
                var steamApp = (SteamApp)e.Parameter;
                url = String.Format(PATTERN_STEAM_APP, steamApp.Id);
            }
            else if (e.Parameter is Card)
            {
                var card = (Card)e.Parameter;
                var encoded = EncodeMarketHashNameForUrl(card.MarketHashName);
                url = String.Format(PATTERN_CARD, card.AppId, encoded);
            }

            if (url != null)
            {
                try
                {
                    System.Diagnostics.Process.Start(url);
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
        }

        private void OpenPostHistory_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenPostHistory_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.PostHistory);
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void OpenProfilePage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenProfilePage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.CommunityPage);
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void OpenSceInvPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is Card || e.Parameter is SteamApp;
        }

        private void OpenSceInvPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "https://www.steamcardexchange.net/index.php?inventorygame-appid-{0}";
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

        private void OpenSentOffers_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenSentOffers_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.SentOffers);
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void OpenStorePage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamApp || e.Parameter is Card;
        }

        private void OpenStorePage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "https://store.steampowered.com/app/{0}/";
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

        private void OpenTradeTopics_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenTradeTopics_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.TradeTopics);
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void OpenTradingForum_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is Card || e.Parameter is SteamApp;
        }

        private void OpenTradingForum_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            const string PATTERN = "https://steamcommunity.com/app/{0}/tradingforum/";
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

        private void OpenUserBadges_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SteamProfileKey;
        }

        private void OpenUserBadges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SteamProfileKey)
            {
                var url = SteamDataDownloader.GetProfileUri((SteamProfileKey)e.Parameter, SteamUrlPattern.Badges);
                System.Diagnostics.Process.Start(url.ToString());
            }
        }

        private void SelectCardsFromOffer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_inventoryLoadWorker != null && _inventoryLoadWorker.IsBusy)
                e.CanExecute = false;
            else
                e.CanExecute = SteamApps != null && SteamApps.Count > 0;
        }

        private void SelectCardsFromOffer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var form = new CardsFromOfferSelect();
            Grid.SetRowSpan(form, mainGrid.RowDefinitions.Count);
            Grid.SetColumnSpan(form, mainGrid.ColumnDefinitions.Count);
            mainGrid.Children.Add(form);
            form.SteamApps = SteamApps;
            form.OwnerProfile = OwnerProfile;
        }

        private void ShowHideCards_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is CardsCompareManager.ShowHideStrategy)
            {
                var steamApps = SteamApps;
                e.CanExecute = steamApps != null && steamApps.Count > 0;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ShowHideCards_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _cardsCompareManager.ShowHideCards((CardsCompareManager.ShowHideStrategy)e.Parameter);

            var cvs = (CollectionViewSource)this.FindResource("steamAppsView");
            cvs.View.Refresh();
        }

        private void SortCards_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is SteamAppSort)
            {
                var steamApps = SteamApps;
                e.CanExecute = steamApps != null && steamApps.Count > 0;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void SortCards_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sortValue = (SteamAppSort)e.Parameter;

            var cvs = (CollectionViewSource)this.FindResource("steamAppsView");
            ((ListCollectionView)cvs.View).CustomSort = new SteamAppComparer(sortValue);
        }

        #endregion Commands

        #region InventoryLoadWorker

        private void InventoryLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (InventoryLoadWorkerArgs)e.Argument;
            var ownerInv = DataManager.GetSteamUserInventory(args.OwnerProfile, args.ForceRefresh);
            var secondInv = DataManager.GetSteamUserInventory(args.SecondProfile, args.ForceRefresh);
            e.Result = new InventoryLoadWorkerResult() { OwnerInv = ownerInv, SecondInv = secondInv };
        }

        private void InventoryLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    ShowException(e.Error);
                    return;
                }

                var result = (InventoryLoadWorkerResult)e.Result;
                if (result != null)
                {
                    MakeErrorMessage(result);

                    if (result.OwnerInv.IsInventoryAvailable && result.SecondInv.IsInventoryAvailable)
                    {
                        if (result.OwnerInv.SteamId == result.SecondInv.SteamId)
                        {
                            _cardsCompareManager.Fill(result.OwnerInv.Cards, new List<Card>());
                            _cardsCompareManager.ShowHideCards(CardsCompareManager.ShowMyCardsStrategy);
                            var steamApps = new List<SteamApp>(_cardsCompareManager.SteamApps.Count);
                            steamApps.AddRange(_cardsCompareManager.SteamApps);
                            SteamApps = steamApps;
                            CompareWithSelf = true;
                        }
                        else
                        {
                            _cardsCompareManager.Fill(result.OwnerInv.Cards, result.SecondInv.Cards);
                            _cardsCompareManager.ShowHideCards(CardsCompareManager.ShowTradeSugestionsStrategy);
                            var steamApps = new List<SteamApp>(_cardsCompareManager.SteamApps.Count);
                            steamApps.AddRange(_cardsCompareManager.SteamApps);
                            SteamApps = steamApps;
                            CompareWithSelf = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void MakeErrorMessage(InventoryLoadWorkerResult result)
        {
            if (result.OwnerInv.ErrorMessage == null && result.SecondInv.ErrorMessage == null)
                return;

            var sb = new StringBuilder();
            if (result.OwnerInv.ErrorMessage != null)
                sb.AppendFormat(Strings.OwnerInvErrorMessage, result.OwnerInv.ErrorMessage).AppendLine();
            if (result.SecondInv.ErrorMessage != null)
                sb.AppendFormat(Strings.SecondInvErrorMessage, SecondProfile.Name, result.SecondInv.ErrorMessage).AppendLine();

            MessageBox.Show(sb.ToString(), Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void RunInventoryLoadWorker(SteamProfile ownerProfile, SteamProfile secondProfile, bool forceRefresh)
        {
            var args = new InventoryLoadWorkerArgs()
            {
                OwnerProfile = ownerProfile,
                SecondProfile = secondProfile,
                ForceRefresh = forceRefresh
            };

            _inventoryLoadWorker.RunWorkerAsync(args);
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

        #endregion InventoryLoadWorker

        private class CompareConverter : DependencyObject, IValueConverter
        {
            private readonly Func<object> _target;

            public CompareConverter(Func<object> target)
            {
                _target = target;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                    return DependencyProperty.UnsetValue;

                if (!typeof(bool).Equals(targetType))
                    throw new NotSupportedException();

                return Object.Equals(value, _target());
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
