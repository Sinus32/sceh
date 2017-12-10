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
        private static RoutedCommand _compareCommand, _editNoteCommand, _showHideCardsCommand,
            _openMarketPageCommand, _openStorePageCommand, _openBadgePageCommand, _openTradingForumCommand,
            _openProfilePageCommand, _openInventoryPageCommand, _openUserBadgesCommand,
            _copyNameCommand, _loginCommand;

        static ScehCommands()
        {
            _loginCommand = new RoutedCommand("Login", typeof(ScehCommands));
            _compareCommand = new RoutedCommand("Compare", typeof(ScehCommands));
            _editNoteCommand = new RoutedCommand("EditNote", typeof(ScehCommands));
            _showHideCardsCommand = new RoutedCommand("ShowHideCards", typeof(ScehCommands));
            _openMarketPageCommand = new RoutedCommand("OpenMarketPage", typeof(ScehCommands));
            _openStorePageCommand = new RoutedCommand("OpenStorePage", typeof(ScehCommands));
            _openBadgePageCommand = new RoutedCommand("OpenBadgePage", typeof(ScehCommands));
            _openTradingForumCommand = new RoutedCommand("OpenTradingForum", typeof(ScehCommands));
            _openProfilePageCommand = new RoutedCommand("OpenProfilePage", typeof(ScehCommands));
            _openInventoryPageCommand = new RoutedCommand("OpenInventoryPage", typeof(ScehCommands));
            _openUserBadgesCommand = new RoutedCommand("OpenUserBadges", typeof(ScehCommands));
            _copyNameCommand = new RoutedCommand("CopyName", typeof(ScehCommands));
        }

        public static RoutedCommand CompareCommand
        {
            get { return _compareCommand; }
        }

        public static RoutedCommand CopyNameCommand
        {
            get { return _copyNameCommand; }
        }

        public static RoutedCommand EditNoteCommand
        {
            get { return _editNoteCommand; }
        }

        public static RoutedCommand LoginCommand
        {
            get { return _loginCommand; }
        }

        public static RoutedCommand OpenBadgePageCommand
        {
            get { return _openBadgePageCommand; }
        }

        public static RoutedCommand OpenInventoryPageCommand
        {
            get { return _openInventoryPageCommand; }
        }

        public static RoutedCommand OpenMarketPageCommand
        {
            get { return _openMarketPageCommand; }
        }

        public static RoutedCommand OpenProfilePageCommand
        {
            get { return _openProfilePageCommand; }
        }

        public static RoutedCommand OpenStorePageCommand
        {
            get { return _openStorePageCommand; }
        }

        public static RoutedCommand OpenTradingForumCommand
        {
            get { return _openTradingForumCommand; }
        }

        public static RoutedCommand OpenUserBadgesCommand
        {
            get { return _openUserBadgesCommand; }
        }

        public static RoutedCommand ShowHideCardsCommand
        {
            get { return _showHideCardsCommand; }
        }
    }
}