using s32.Sceh.DataModel;
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

        private void PasteCards_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SteamApps != null && e.Parameter is ScehCommands.CardSelection;
        }

        private void PasteCards_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (SteamApps == null || !(e.Parameter is ScehCommands.CardSelection))
                return;

            var cardSelection = (ScehCommands.CardSelection)e.Parameter;

            var sb = new StringBuilder();

            foreach (var app in SteamApps)
            {
                bool flag = true;

                IEnumerable<Card> cards;
                switch (cardSelection)
                {
                    case ScehCommands.CardSelection.MyCards:
                        cards = app.MyCards;
                        break;

                    case ScehCommands.CardSelection.OtherCards:
                        cards = app.OtherCards;
                        break;

                    default:
                        return;
                }

                foreach (var card in cards)
                {
                    if (!card.IsSelected)
                        continue;

                    if (flag)
                    {
                        sb.Append(' ').Append(app.Name);
                        flag = false;
                    }

                    sb.Append(' ').Append(card.Name);
                }
            }
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