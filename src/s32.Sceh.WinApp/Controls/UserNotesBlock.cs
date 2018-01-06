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
using s32.Sceh.BBCode;
using s32.Sceh.DataModel;
using s32.Sceh.UserNoteTags;

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

        private void PrintComplexNode(List<Inline> inlines, BBNode node)
        {
            switch (node.NodeType)
            {
                case BBNodeType.Root:
                    foreach (var subNode in node.Content)
                        PrintComplexNode(inlines, subNode);
                    break;

                case BBNodeType.Text:
                    inlines.Add(new Run(node.GetText()));
                    break;

                case BBNodeType.TagStart:
                    if (!PrintTagNode(inlines, (IBBTagNode) node))
                        PrintUnknowNode(inlines, node);
                    break;

                default:
                    PrintUnknowNode(inlines, node);
                    break;
            }
        }

        private bool PrintTagNode(List<Inline> inlines, IBBTagNode node)
        {
            throw new NotImplementedException();
        }

        private void PrintUnknowNode(List<Inline> inlines, BBNode node)
        {
            if (node.Content.Count > 0)
            {
                var subInlines = new List<Inline>();
                subInlines.Add(new Run(node.GetText()) { FontWeight = FontWeights.Bold });

                foreach (var subNode in node.Content)
                    PrintComplexNode(subInlines, subNode);

                var span = new Span();
                span.Inlines.AddRange(subInlines);
                inlines.Add(span);
            }
            else
            {
                inlines.Add(new Run(node.GetText()) { FontWeight = FontWeights.Bold });
            }
        }

        private void SetInlines(IEnumerable<string> texts)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var lexer = new BBCodeLexer();
            var parser = new BBCodeParser();
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
                PrintComplexNode(inlines, rootNode);
                inlines.Add(new Run(Environment.NewLine));
            }

            System.Diagnostics.Debug.WriteLine("ParseString: {0}", stopwatch.Elapsed);

            Inlines.Clear();
            Inlines.AddRange(inlines);

            System.Diagnostics.Debug.WriteLine("AddRange: {0}", stopwatch.Elapsed);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            if (IsLoaded)
                UpdateUserNotes(UserNotes);
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