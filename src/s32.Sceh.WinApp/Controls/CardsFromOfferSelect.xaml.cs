using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace s32.Sceh.WinApp.Controls
{
    /// <summary>
    /// Interaction logic for CardsFromOfferSelect.xaml
    /// </summary>
    public partial class CardsFromOfferSelect : UserControl
    {
        public CardsFromOfferSelect()
        {
            InitializeComponent();
        }

        public List<SteamApp> SteamApps { get; internal set; }
        public SteamProfile OwnerProfile { get; internal set; }

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

        /*
            var data = Clipboard.GetDataObject();
            if (data != null)
            {
                var formats = data.GetFormats(false);
                MessageBox.Show("Formats: " + String.Join(", ", formats));

                if (formats.Contains(DataFormats.Html))
                {
                    var html = data.GetData(DataFormats.Html);
                    if (html != null)
                    {
                        MessageBox.Show(html.ToString(), html.GetType().Name);
                    }
                }
            }
            else
            {
                MessageBox.Show("No data");
            }
            */

        #region Commands

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveSelfFromParent();
        }

        private void Ok_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void Ok_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveSelfFromParent();
        }

        private void OpenPageButton_Click(object sender, RoutedEventArgs e)
        {
            var self = (Button)sender;
            self.ContextMenu.IsEnabled = true;
            self.ContextMenu.PlacementTarget = self;
            self.ContextMenu.Placement = PlacementMode.Bottom;
            self.ContextMenu.IsOpen = true;
        }

        #endregion Commands
    }
}