using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class BBCodeBlock : TextBlock
    {
        public static readonly DependencyProperty BBCodeTextProperty =
            DependencyProperty.Register("BBCodeText", typeof(string), typeof(BBCodeBlock), new PropertyMetadata(null, BBCodeTextChanged));

        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(int), typeof(BBCodeBlock), new PropertyMetadata(0));

        private readonly DispatcherTimer _timer;
        private bool _isActive, _needRefresh;

        public BBCodeBlock()
        {
            _timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher);
            _timer.Tick += Timer_Tick;

            Loaded += UserNotesBlock_Loaded;
            Unloaded += UserNotesBlock_Unloaded;
        }

        ~BBCodeBlock()
        {
            Dispose(false);
        }

        public string BBCodeText
        {
            get { return (string)GetValue(BBCodeTextProperty); }
            set { SetValue(BBCodeTextProperty, value); }
        }

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdateInlines()
        {
            if (_needRefresh)
                _needRefresh = false;
            else
                return;

            var input = BBCodeText;
            if (input == null)
            {
                Inlines.Clear();
                return;
            }
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var lexer = new BBCodeLexer();
            var parser = new BBCodeParser();
            var inlines = new List<Inline>();
            var tokens = lexer.ParseString(input);
            var rootNode = parser.Parse(tokens);

            PrintComplexNode(inlines, rootNode);

            System.Diagnostics.Debug.WriteLine("ParseString: {0}", stopwatch.Elapsed);

            Inlines.Clear();
            Inlines.AddRange(inlines);

            System.Diagnostics.Debug.WriteLine("AddRange: {0}", stopwatch.Elapsed);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= Timer_Tick;
                }
            }
        }

        private static void BBCodeTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (BBCodeBlock)d;
            self._needRefresh = true;
            if (self._isActive)
                if (!self.SetupTimer())
                    self.UpdateInlines();
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
            if (node.IsSelfClosed)
            {
                switch (node.TagName)
                {
                    case "->":
                        var rightArrowTemplate = (ControlTemplate)FindResource("rightArrowTemplate");
                        var rightArrow = (FrameworkElement)rightArrowTemplate.LoadContent();
                        rightArrow.Height = this.FontSize;
                        inlines.Add(new InlineUIContainer(rightArrow) { BaselineAlignment = BaselineAlignment.Center });
                        foreach (var subNode in node.Content)
                            PrintComplexNode(inlines, subNode);
                        return true;

                    case "br":
                        inlines.Add(new LineBreak());
                        return true;

                    default:
                        return false;
                }
            }
            else
            {
                Span span;
                switch (node.TagName)
                {
                    case "b":
                        span = new Span() { FontWeight = FontWeights.Bold };
                        break;

                    case "i":
                        span = new Span() { FontStyle = FontStyles.Italic };
                        break;

                    case "u":
                        span = new Span() { TextDecorations = System.Windows.TextDecorations.Underline };
                        break;

                    case "s":
                        span = new Span() { TextDecorations = System.Windows.TextDecorations.Strikethrough };
                        break;

                    case "c":
                        if (String.IsNullOrWhiteSpace(node.TagParam))
                            return false;
                        var brush = typeof(Brushes).GetProperty(node.TagParam.Trim(), BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly);
                        if (brush == null)
                            return false;
                        span = new Span() { Foreground = (Brush)brush.GetValue(null) };
                        break;

                    default:
                        return false;
                }

                if (node.Content.Count == 0)
                    return true; //empty, but handled

                inlines.Add(span);
                var subInlines = new List<Inline>();
                foreach (var subNode in node.Content)
                    if (!Object.ReferenceEquals(node.SecondTag, subNode))
                        PrintComplexNode(subInlines, subNode);

                span.Inlines.AddRange(subInlines);
                return true;
            }
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

        private bool SetupTimer()
        {
            var delay = Delay;
            if (delay > 0)
            {
                _timer.Interval = TimeSpan.FromMilliseconds(delay);
                _timer.Start();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            if (IsLoaded && _isActive)
                UpdateInlines();
        }

        private void UserNotesBlock_Loaded(object sender, RoutedEventArgs e)
        {
            _isActive = true;
            if (!SetupTimer())
                UpdateInlines();
        }

        private void UserNotesBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            _isActive = false;
            _timer.Stop();
        }
    }
}