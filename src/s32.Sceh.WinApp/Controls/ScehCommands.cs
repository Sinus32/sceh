﻿using System;
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
        private static RoutedCommand _ok, _cancel, _compare, _editNote, _showHideCards,
            _openMarketPage, _openStorePage, _openBadgePage, _openTradingForum,
            _openProfilePage, _openInventoryPage, _openUserBadges,
            _openTradeOffers, _openTradeTopics, _openPostHistory,
            _copyName, _scoreUpDown, _pasteCards;

        static ScehCommands()
        {
            _ok = new RoutedCommand("Ok", typeof(ScehCommands));
            _cancel = new RoutedCommand("Cancel", typeof(ScehCommands));
            _compare = new RoutedCommand("Compare", typeof(ScehCommands));
            _editNote = new RoutedCommand("EditNote", typeof(ScehCommands));
            _showHideCards = new RoutedCommand("ShowHideCards", typeof(ScehCommands));
            _openMarketPage = new RoutedCommand("OpenMarketPage", typeof(ScehCommands));
            _openStorePage = new RoutedCommand("OpenStorePage", typeof(ScehCommands));
            _openBadgePage = new RoutedCommand("OpenBadgePage", typeof(ScehCommands));
            _openTradingForum = new RoutedCommand("OpenTradingForum", typeof(ScehCommands));
            _openProfilePage = new RoutedCommand("OpenProfilePage", typeof(ScehCommands));
            _openInventoryPage = new RoutedCommand("OpenInventoryPage", typeof(ScehCommands));
            _openUserBadges = new RoutedCommand("OpenUserBadges", typeof(ScehCommands));
            _openTradeOffers = new RoutedCommand("OpenTradeOffers", typeof(ScehCommands));
            _openTradeTopics = new RoutedCommand("OpenTradeTopics", typeof(ScehCommands));
            _openPostHistory = new RoutedCommand("OpenPostHistory", typeof(ScehCommands));
            _copyName = new RoutedCommand("CopyName", typeof(ScehCommands));
            _scoreUpDown = new RoutedCommand("ScoreUpDown", typeof(ScehCommands));
            _pasteCards = new RoutedCommand("PasteCards", typeof(ScehCommands));
        }

        public enum CardSelection
        {
            None,

            MyCards,

            OtherCards,
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

        public static RoutedCommand EditNote
        {
            get { return _editNote; }
        }

        public static RoutedCommand Ok
        {
            get { return _ok; }
        }

        public static RoutedCommand OpenBadgePage
        {
            get { return _openBadgePage; }
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

        public static RoutedCommand OpenStorePage
        {
            get { return _openStorePage; }
        }

        public static RoutedCommand OpenTradeOffers
        {
            get { return _openTradeOffers; }
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

        public static RoutedCommand PasteCards
        {
            get { return _pasteCards; }
        }

        public static RoutedCommand ScoreUpDown
        {
            get { return _scoreUpDown; }
        }

        public static RoutedCommand ShowHideCards
        {
            get { return _showHideCards; }
        }
    }
}