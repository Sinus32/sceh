using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using s32.Sceh.DataModel;
using s32.Sceh.UserNoteTags;
using s32.Sceh.WinApp.BBCodeWriters;
using s32.Sceh.WinApp.Code;
using s32.Sceh.WinApp.Translations;

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
        private static readonly Regex ValidTradeUrlRe = new Regex(@"^https://steamcommunity.com/tradeoffer/new/\?partner=[0-9]{5,10}&token=[a-zA-Z0-9_-]{5,12}$", RegexOptions.None);

        public ProfileNoteEditor()
        {
            InitializeComponent();

            var brushArray = BrushName.GetBrushArray();
            ColorList = new List<TextColorWriter>(brushArray.Length);
            ColorList.AddRange(brushArray.Select(q => new TextColorWriter(q)));

            DataContext = this;

            Loaded += ProfileNoteEditor_Loaded;
        }

        public bool AutoFocus
        {
            get { return (bool)GetValue(AutoFocusProperty); }
            set { SetValue(AutoFocusProperty, value); }
        }

        public List<TextColorWriter> ColorList { get; set; }

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

        private void ColorSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = VisualTreeHelper.GetParent((DependencyObject)sender);
            while (popup != null && !(popup is Popup))
                popup = LogicalTreeHelper.GetParent(popup) ?? VisualTreeHelper.GetParent(popup);

            if (popup != null)
                ((Popup)popup).IsOpen = false;
        }

        private void ProfileNoteEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Load(Source);
            if (AutoFocus)
                Keyboard.Focus(tbEditor);
        }

        private bool RemoveSelfFromParent()
        {
            var parent = LogicalTreeHelper.GetParent(this);
            if (parent is Panel)
            {
                var panel = (Panel)parent;
                panel.Children.Remove(this);
                return true;
            }
            return false;
        }

        #region Commands

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveSelfFromParent();
        }

        private void PasteTag_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(e.Parameter is IBBCodeWriter))
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = ((IBBCodeWriter)e.Parameter).CanWrite(SteamApps);
        }

        private void PasteTag_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(e.Parameter is IBBCodeWriter))
                return;

            ((IBBCodeWriter)e.Parameter).Write(tbEditor, SteamApps);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Source != null;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Save(Source))
                RemoveSelfFromParent();
        }

        #endregion Commands
    }
}
