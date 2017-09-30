using s32.Sceh.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s32.Sceh.WinApp.Forms
{
    public partial class InvCompareForm : Form
    {
        public InvCompareForm()
        {
            InitializeComponent();
        }

        public void SetSteamUser(SteamUser steamUser)
        {
            pvCurrent.SteamUser = steamUser;
            icInventories.SteamUser = steamUser;
        }

        private void changeUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}