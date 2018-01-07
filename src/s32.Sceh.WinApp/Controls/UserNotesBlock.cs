using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            DependencyProperty.Register("UserNotes", typeof(string), typeof(UserNotesBlock), new PropertyMetadata(null, UserNotesChanged));

        private static readonly Dictionary<string, Action<Inline>> _formatters;
        private DispatcherTimer _timer;

        static UserNotesBlock()
        {
            _formatters = new Dictionary<string, Action<Inline>>();
            _formatters.Add("b", q => q.FontWeight = FontWeights.Bold);
            _formatters.Add("i", q => q.FontStyle = FontStyles.Italic);
            _formatters.Add("u", q => q.TextDecorations = System.Windows.TextDecorations.Underline);
            _formatters.Add("s", q => q.TextDecorations = System.Windows.TextDecorations.Strikethrough);
        }

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

        public string UserNotes
        {
            get { return (string)GetValue(UserNotesProperty); }
            set { SetValue(UserNotesProperty, value); }
        }

        public void UpdateUserNotes(string userNotes)
        {
            if (userNotes == null)
            {
                Inlines.Clear();
                return;
            }
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var lexer = new BBCodeLexer();
            var parser = new BBCodeParser();
            var inlines = new List<Inline>();
            var tokens = lexer.ParseString(userNotes);
            var rootNode = parser.Parse(tokens);

            PrintComplexNode(inlines, rootNode);

            System.Diagnostics.Debug.WriteLine("ParseString: {0}", stopwatch.Elapsed);

            Inlines.Clear();
            Inlines.AddRange(inlines);

            System.Diagnostics.Debug.WriteLine("AddRange: {0}", stopwatch.Elapsed);
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
                    if (!PrintTagNode(inlines, (IBBTagNode)node))
                        PrintUnknowNode(inlines, node);
                    break;

                default:
                    PrintUnknowNode(inlines, node);
                    break;
            }
        }

        private bool PrintFormattingTag(List<Inline> inlines, IBBTagNode node)
        {
            if (node.TagName == "->")
            {
                var rightArrowTemplate = (ControlTemplate)FindResource("rightArrowTemplate");
                var rightArrow = (FrameworkElement)rightArrowTemplate.LoadContent();
                rightArrow.Height = this.FontSize;
                inlines.Add(new InlineUIContainer(rightArrow) { BaselineAlignment = BaselineAlignment.Center });
                foreach (var subNode in node.Content)
                    PrintComplexNode(inlines, subNode);
                return true;
            }

            if (!node.IsSelfClosed)
            {
                Action<Inline> formatter;
                if (_formatters.TryGetValue(node.TagName, out formatter))
                {
                    if (node.Content.Count == 0)
                        return true; //empty, but handled

                    var span = new Span();
                    formatter(span);
                    var subInlines = new List<Inline>();
                    foreach (var subNode in node.Content)
                        if (!Object.ReferenceEquals(node.SecondTag, subNode))
                            PrintComplexNode(subInlines, subNode);
                    span.Inlines.AddRange(subInlines);
                    inlines.Add(span);
                    return true;
                }
            }

            return false;
        }

        private void PrintKnowTag(List<Inline> inlines, IUserNoteTag userNoteTag)
        {
            BitmapImage icon;
            Run text;
            switch (userNoteTag.Name)
            {
                case DateTimeTag.TagName:
                    icon = null;
                    text = new Run(userNoteTag.GetFormatedText()) { Foreground = Brushes.Maroon, FontWeight = FontWeights.Bold };
                    break;

                case SteamAppTag.TagName:
                    icon = null;
                    //icon = (BitmapImage)FindResource("gameInline");
                    text = new Run(userNoteTag.GetFormatedText()) { Foreground = Brushes.Purple };
                    break;

                case SteamCardTag.TagName:
                    icon = null;
                    //icon = (BitmapImage)FindResource("cardInline");
                    text = new Run(userNoteTag.GetFormatedText()) { Foreground = Brushes.Navy, FontWeight = FontWeights.Bold };
                    break;

                default:
                    icon = null;
                    text = new Run(userNoteTag.GetFormatedText()) { Foreground = Brushes.Goldenrod };
                    break;
            }

            if (icon != null)
            {
                var img = new Image();
                img.BeginInit();
                img.Height = 16;
                img.Source = icon;
                img.EndInit();
                inlines.Add(new InlineUIContainer(img) { BaselineAlignment = BaselineAlignment.Center });
            }

            inlines.Add(text);
        }

        private bool PrintTagNode(List<Inline> inlines, IBBTagNode node)
        {
            if (!node.IsValid || node.SecondTag == null || !node.SecondTag.IsValid)
                return false;

            IUserNoteTag userNoteTag = UserNoteTagFactory.Instance.CreateTag(node);
            if (userNoteTag != null)
            {
                PrintKnowTag(inlines, userNoteTag);
                return true;
            }

            return PrintFormattingTag(inlines, node);
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