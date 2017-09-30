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
using System.Windows.Shapes;
using s32.Sceh.Data;

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for InvCompareWindow.xaml
    /// </summary>
    public partial class InvCompareWindow : Window
    {
        public static readonly DependencyProperty OwnerProfileProperty =
            DependencyProperty.Register("OwnerProfile", typeof(SteamProfile), typeof(InvCompareWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SecondProfileProperty =
            DependencyProperty.Register("SecondProfile", typeof(SteamProfile), typeof(InvCompareWindow), new PropertyMetadata(null));

        public InvCompareWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public SteamProfile OwnerProfile
        {
            get { return (SteamProfile)GetValue(OwnerProfileProperty); }
            set { SetValue(OwnerProfileProperty, value); }
        }

        public SteamProfile SecondProfile
        {
            get { return (SteamProfile)GetValue(SecondProfileProperty); }
            set { SetValue(SecondProfileProperty, value); }
        }
    }
}