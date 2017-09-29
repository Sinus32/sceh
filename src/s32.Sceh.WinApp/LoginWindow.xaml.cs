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

namespace s32.Sceh.WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<SpecialFolder> _folders;

        public MainWindow()
        {
            InitializeComponent();

            _folders = new List<SpecialFolder>();
            foreach (Environment.SpecialFolder dt in Enum.GetValues(typeof(Environment.SpecialFolder)))
                _folders.Add(new SpecialFolder(dt));
            _folders.Sort((a, b) => String.Compare(a.Name, b.Name));

            DataContext = this;
        }

        public List<SpecialFolder> Folders
        {
            get { return _folders; }
        }

        public class SpecialFolder
        {
            private Environment.SpecialFolder _sf;
            private string _name;
            private string _path;

            public SpecialFolder(Environment.SpecialFolder sf)
            {
                _sf = sf;
                _name = Enum.GetName(typeof(Environment.SpecialFolder), _sf);
                _path = Environment.GetFolderPath(_sf);
            }

            public string Name
            {
                get { return _name; }
            }

            public string Path
            {
                get { return _path; }
            }
        }
    }
}
