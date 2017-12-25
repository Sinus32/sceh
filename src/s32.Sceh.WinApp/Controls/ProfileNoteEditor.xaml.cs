using s32.Sceh.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public bool Save(UserNotes Source)
        {
            throw new NotImplementedException();
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

        private void RaiseCancelCommand()
        {
            RoutedCommand routed = ScehCommands.CancelCommand;
            IInputElement target = this;
            object parameter = null;

            if (routed.CanExecute(parameter, target))
                routed.Execute(parameter, target);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Save(Source))
                Dispatcher.Invoke(RaiseCancelCommand, DispatcherPriority.Background);
        }

        #endregion Commands
    }
}