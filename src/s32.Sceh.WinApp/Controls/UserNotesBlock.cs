using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using s32.Sceh.DataModel;
using s32.Sceh.UserNoteTags;
using s32.Sceh.UserNoteTags.Lexer;
using s32.Sceh.UserNoteTags.Parser;

namespace s32.Sceh.WinApp.Controls
{
    public class UserNotesBlock : TextBlock
    {
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(int), typeof(UserNotesBlock), new PropertyMetadata(100));

        public static readonly DependencyProperty UserNotesProperty =
            DependencyProperty.Register("UserNotes", typeof(object), typeof(UserNotesBlock), new PropertyMetadata(null, UserNotesChanged));

        private DispatcherTimer _timer;

        public UserNotesBlock()
        {
            Loaded += UserNotesBlock_Loaded;
            Unloaded += UserNotesBlock_Unloaded;
        }

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public object UserNotes
        {
            get { return (object)GetValue(UserNotesProperty); }
            set { SetValue(UserNotesProperty, value); }
        }

        public void UpdateUserNotes(object userNotes)
        {
            if (userNotes == null)
            {
                Inlines.Clear();
                return;
            }

            if (userNotes is string)
            {
                var texts = ((string)userNotes).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                SetInlines(texts);
                return;
            }

            if (userNotes is UserNotes)
            {
                var texts = ((UserNotes)userNotes).Select(q => q.Text);
                SetInlines(texts);
                return;
            }
        }

        private static void UserNotesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (UserNotesBlock)d;
            if (self._timer != null)
            {
                self._timer.Stop();
                self._timer.Start();
            }
        }

        private void SetInlines(IEnumerable<string> texts)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var lexer = new NoteLexer();
            var parser = new NoteParser();
            var inlines = new List<Inline>();

            foreach (var text in texts)
            {
                var tokens = lexer.ParseString(text);
                var rootNode = parser.Parse(tokens);
                //foreach (var token in tokens)
                //{
                //    inlines.Add(new Run(TokenTag(token.TokenType) + "{") { Foreground = Brushes.DarkRed });
                //    inlines.Add(new Run(token.Content) { FontWeight = FontWeights.Bold });
                //    inlines.Add(new Run("} ") { Foreground = Brushes.DarkRed });
                //}
                CollectInlines(inlines, rootNode);
                inlines.Add(new Run(Environment.NewLine));
            }

            System.Diagnostics.Debug.WriteLine("ParseString: {0}", stopwatch.Elapsed);

            Inlines.Clear();
            Inlines.AddRange(inlines);

            System.Diagnostics.Debug.WriteLine("AddRange: {0}", stopwatch.Elapsed);
        }

        private void CollectInlines(List<Inline> inlines, Node node)
        {
            inlines.Add(new Run(NodeTag(node.NodeType) + "{") { Foreground = Brushes.DarkRed });
            inlines.Add(new Run(node.GetText()) { FontWeight = FontWeights.Bold });

            foreach (var subNode in node.Content)
            {
                inlines.Add(new Run(" "));
                CollectInlines(inlines, subNode);
            }

            inlines.Add(new Run("}") { Foreground = Brushes.DarkRed });
        }

        private string NodeTag(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Root: return "r";
                case NodeType.Text: return "t";
                case NodeType.TagStart: return "s";
                case NodeType.TagEnd: return "e";
                default: return "";
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            if (IsLoaded)
                UpdateUserNotes(UserNotes);
        }

        private string TokenTag(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Text: return "t";
                case TokenType.BeginTag: return "b";
                case TokenType.EndTag: return "e";
                case TokenType.TagClose: return "c";
                case TokenType.TagName: return "n";
                case TokenType.Separator: return "s";
                case TokenType.TagParam: return "p";
                default: return "";
            }
        }

        private void UserNotesBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher);
                var delay = Delay;
                if (delay < 0)
                    delay = 0;
                _timer.Interval = TimeSpan.FromMilliseconds(delay);
                _timer.Tick += Timer_Tick;
            }
            _timer.Start();
        }

        private void UserNotesBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }
    }
}