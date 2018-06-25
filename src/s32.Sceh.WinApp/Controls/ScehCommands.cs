using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace s32.Sceh.WinApp.Controls
{
    public class ScehCommands : DependencyObject
    {
        private static readonly RoutedCommand _ok, _cancel, _compare, _editNote,
            _showHideCards, _sortCards, _selectCardsFromOffer, _deselectCards,
            _openMarketPage, _openAllMarketPages, _openStorePage, _openBadgePage,
            _openTradingForum, _openProfilePage, _openInventoryPage, _openUserBadges,
            _openIncomingOffers, _openSentOffers, _openTradeTopics, _openPostHistory,
            _openSceInvPage,
            _makeOffer, _copyName, _copyNamePlus, _pasteTag;

        static ScehCommands()
        {
            _ok = new RoutedCommand("Ok", typeof(ScehCommands));
            _cancel = new RoutedCommand("Cancel", typeof(ScehCommands));
            _compare = new RoutedCommand("Compare", typeof(ScehCommands));
            _editNote = new RoutedCommand("EditNote", typeof(ScehCommands));
            _showHideCards = new RoutedCommand("ShowHideCards", typeof(ScehCommands));
            _sortCards = new RoutedCommand("SortCards", typeof(ScehCommands));
            _deselectCards = new RoutedCommand("DeselectCards", typeof(ScehCommands));
            _openMarketPage = new RoutedCommand("OpenMarketPage", typeof(ScehCommands));
            _openAllMarketPages = new RoutedCommand("OpenAllMarketPages", typeof(ScehCommands));
            _openStorePage = new RoutedCommand("OpenStorePage", typeof(ScehCommands));
            _openBadgePage = new RoutedCommand("OpenBadgePage", typeof(ScehCommands));
            _openTradingForum = new RoutedCommand("OpenTradingForum", typeof(ScehCommands));
            _openProfilePage = new RoutedCommand("OpenProfilePage", typeof(ScehCommands));
            _openInventoryPage = new RoutedCommand("OpenInventoryPage", typeof(ScehCommands));
            _openUserBadges = new RoutedCommand("OpenUserBadges", typeof(ScehCommands));
            _openIncomingOffers = new RoutedCommand("OpenIncomingOffers", typeof(ScehCommands));
            _openSentOffers = new RoutedCommand("OpenSentOffers", typeof(ScehCommands));
            _openTradeTopics = new RoutedCommand("OpenTradeTopics", typeof(ScehCommands));
            _openPostHistory = new RoutedCommand("OpenPostHistory", typeof(ScehCommands));
            _openSceInvPage = new RoutedCommand("OpenSceInvPage", typeof(ScehCommands));
            _makeOffer = new RoutedCommand("MakeOffer", typeof(ScehCommands));
            _copyName = new RoutedCommand("CopyName", typeof(ScehCommands));
            _copyNamePlus = new RoutedCommand("CopyNamePlus", typeof(ScehCommands));
            _pasteTag = new RoutedCommand("PasteTag", typeof(ScehCommands));
            _selectCardsFromOffer = new RoutedCommand("SelectCardsFromOffer", typeof(ScehCommands));
        }

        public static RoutedCommand Cancel
        {
            get { return _cancel; }
        }

        public static RoutedCommand Compare
        {
            get { return _compare; }
        }

        public static RoutedCommand CopyName
        {
            get { return _copyName; }
        }

        public static RoutedCommand CopyNamePlus
        {
            get { return _copyNamePlus; }
        }

        public static RoutedCommand DeselectCards
        {
            get { return _deselectCards; }
        }

        public static RoutedCommand EditNote
        {
            get { return _editNote; }
        }

        public static RoutedCommand MakeOffer
        {
            get { return _makeOffer; }
        }

        public static RoutedCommand Ok
        {
            get { return _ok; }
        }

        public static RoutedCommand OpenAllMarketPages
        {
            get { return _openAllMarketPages; }
        }

        public static RoutedCommand OpenBadgePage
        {
            get { return _openBadgePage; }
        }

        public static RoutedCommand OpenIncomingOffers
        {
            get { return _openIncomingOffers; }
        }

        public static RoutedCommand OpenInventoryPage
        {
            get { return _openInventoryPage; }
        }

        public static RoutedCommand OpenMarketPage
        {
            get { return _openMarketPage; }
        }

        public static RoutedCommand OpenPostHistory
        {
            get { return _openPostHistory; }
        }

        public static RoutedCommand OpenProfilePage
        {
            get { return _openProfilePage; }
        }

        public static RoutedCommand OpenSceInvPage
        {
            get { return _openSceInvPage; }
        }

        public static RoutedCommand OpenSentOffers
        {
            get { return _openSentOffers; }
        }

        public static RoutedCommand OpenStorePage
        {
            get { return _openStorePage; }
        }

        public static RoutedCommand OpenTradeTopics
        {
            get { return _openTradeTopics; }
        }

        public static RoutedCommand OpenTradingForum
        {
            get { return _openTradingForum; }
        }

        public static RoutedCommand OpenUserBadges
        {
            get { return _openUserBadges; }
        }

        public static RoutedCommand PasteTag
        {
            get { return _pasteTag; }
        }

        public static RoutedCommand SelectCardsFromOffer
        {
            get { return _selectCardsFromOffer; }
        }

        public static RoutedCommand ShowHideCards
        {
            get { return _showHideCards; }
        }

        public static RoutedCommand SortCards
        {
            get { return _sortCards; }
        }
    }
}
