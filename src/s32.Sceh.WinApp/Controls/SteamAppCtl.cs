using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using s32.Sceh.Classes;

namespace s32.Sceh.WinApp.Controls
{
    public partial class SteamAppCtl : UserControl
    {
        public SteamAppCtl()
        {
            InitializeComponent();
        }

        public SteamApp SteamApp { get; set; }
    }
}
