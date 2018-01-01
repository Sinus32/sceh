using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using s32.Sceh.DataModel;
using s32.Sceh.WinApp.Translations;
using s32.Sceh.WinApp.UserNoteTags;

namespace s32.Sceh.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for ProfileNoteEditor.xaml
    /// </summary>
    public partial class ProfileNoteEditor : UserControl
    {
        public static readonly DependencyProperty AutoFocusProperty =
            DependencyProperty.Register("AutoFocus", typeof(bool), typeof(ProfileNoteEditor), new PropertyMetadata(null));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(UserNotes), typeof(ProfileNoteEditor), new PropertyMetadata(null));

        public static readonly DependencyProperty SteamAppsProperty =
            DependencyProperty.Register("SteamApps", typeof(List<SteamApp>), typeof(ProfileNoteEditor), new PropertyMetadata(null));

        private static readonly Regex ScoreRe = new Regex(@"^\[([+-]?[0-9]+)\]", RegexOptions.None);
        private static readonly Regex ValidTradeUrlRe = new Regex(@"^https://steamcommunity.com/tradeoffer/new/\?partner=[0-9]{5,10}&token=[a-zA-Z0-9]{5,12}$", RegexOptions.None);

        public ProfileNoteEditor()
        {
            InitializeComponent();
        }

        public bool AutoFocus
        {
            get { return (bool)GetValue(AutoFocusProperty); }
            set { SetValue(AutoFocusProperty, value); }
        }

        public UserNotes Source
        {
            get { return (UserNotes)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public List<SteamApp> SteamApps
        {
            get { return (List<SteamApp>)GetValue(SteamAppsProperty); }
            set { SetValue(SteamAppsProperty, value); }
        }

        public void Load(UserNotes notes)
        {
            if (notes == null)
                return;

            if (notes.TradeUrl != null)
                tbTradeLink.Text = notes.TradeUrl.ToString();
            else
                tbTradeLink.Clear();

            tbEditor.Clear();

            var sb = new StringBuilder();

            foreach (var note in notes)
            {
                if (note.Score != 0)
                    sb.AppendFormat("[{0:+0;-0}] ", note.Score);
                sb.AppendLine(note.Text);
            }

            tbEditor.Text = sb.ToString();
            tbEditor.CaretIndex = sb.Length;
        }

        public bool Save(UserNotes notes)
        {
            if (notes == null)
                return false;

            var tradeUrl = tbTradeLink.Text;
            if (String.IsNullOrEmpty(tradeUrl))
            {
                notes.TradeUrl = null;
            }
            else
            {
                Uri url;
                if (Uri.TryCreate(tradeUrl, UriKind.Absolute, out url))
                {
                    if (url.Scheme != "https")
                    {
                        MessageBox.Show(Strings.InvalidTradeUrlScheme, Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return false;
                    }

                    if (!ValidTradeUrlRe.IsMatch(url.ToString()))
                    {
                        var resp = MessageBox.Show(Strings.StrangeTradeUrl, Strings.WarningTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (resp != MessageBoxResult.Yes)
                            return false;
                    }
                    notes.TradeUrl = url;
                }
                else
                {
                    MessageBox.Show(Strings.InvalidTradeUrl, Strings.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
            }

            notes.Clear();

            var lines = tbEditor.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                int score;
                var text = line.Trim();
                var match = ScoreRe.Match(text);
                if (match.Success)
                {
                    score = Int32.Parse(match.Groups[1].Value);
                    text = text.Substring(match.Value.Length).Trim();
                }
                else
                {
                    score = 0;
                }

                var note = new UserNote(score, text);
                notes.Add(note);
            }
            return true;
        }

        private void noteEditForm_Loaded(object sender, RoutedEventArgs e)
        {
            Load(Source);
            if (AutoFocus)
                Keyboard.Focus(tbEditor);
        }

        #region Commands

        private string BuildCardsTags(Func<SteamApp, bool> getIsSelected, Func<SteamApp, IEnumerable<Card>> getCards)
        {
            var sb = new StringBuilder();

            foreach (var app in SteamApps)
            {
                if (!getIsSelected(app))
                    continue;

                if (sb.Length > 0)
                    sb.Append(' ');
                sb.Append(new SteamAppTag(app));

                foreach (var card in getCards(app))
                {
                    if (!card.IsSelected)
                        continue;

                    if (sb.Length > 0)
                        sb.Append(' ');
                    sb.Append(new SteamCardTag(card));
                }
            }

            return sb.ToString();
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parent = LogicalTreeHelper.GetParent(this);
            if (parent is Panel)
            {
                var panel = (Panel)parent;
                panel.Children.Remove(this);
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void PasteTag_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(e.Parameter is TagSelection))
            {
                e.CanExecute = false;
                return;
            }

            switch ((TagSelection)e.Parameter)
            {
                case TagSelection.Date:
                    e.CanExecute = true;
                    return;

                case TagSelection.MyCards:
                    e.CanExecute = SteamApps != null && SteamApps.Any(steamApp => steamApp.MyIsSelected);
                    return;

                case TagSelection.OtherCards:
                    e.CanExecute = SteamApps != null && SteamApps.Any(steamApp => steamApp.OtherIsSelected);
                    return;

                default:
                    e.CanExecute = false;
                    return;
            }
        }

        private void PasteTag_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(e.Parameter is TagSelection))
                return;

            string text = null;
            switch ((TagSelection)e.Parameter)
            {
                case TagSelection.Date:
                    text = new DateTimeTag(DateTime.Today).BuildTag();
                    break;

                case TagSelection.MyCards:
                    if (SteamApps != null)
                        text = BuildCardsTags(steamApp => steamApp.MyIsSelected, steamApp => steamApp.MyCards);
                    break;

                case TagSelection.OtherCards:
                    if (SteamApps != null)
                        text = BuildCardsTags(steamApp => steamApp.OtherIsSelected, steamApp => steamApp.OtherCards);
                    break;
            }

            if (!String.IsNullOrEmpty(text))
                tbEditor.SelectedText = text;
        }

        private void RaiseCancelCommand()
        {
            RoutedCommand routed = ScehCommands.Cancel;
            IInputElement target = this;
            object parameter = null;

            if (routed.CanExecute(parameter, target))
                routed.Execute(parameter, target);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Source != null;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Save(Source))
                Dispatcher.Invoke(RaiseCancelCommand, DispatcherPriority.Background);
        }

        private void ScoreUpDown_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is int;
        }

        private void ScoreUpDown_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        #endregion Commands
    }
}