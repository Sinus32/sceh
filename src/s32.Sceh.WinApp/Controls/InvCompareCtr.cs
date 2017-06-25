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
using s32.Sceh.Code;

namespace s32.Sceh.WinApp.Controls
{
    public partial class InvCompareCtr : UserControl
    {
        private SteamUser _otherUser;
        private SteamUser _steamUser;

        public InvCompareCtr()
        {
            InitializeComponent();
        }

        public SteamUser SteamUser
        {
            get { return _steamUser; }
            set { SetSteamUser(value); }
        }

        protected void SetOtherUser(SteamUser steamUser)
        {
            _otherUser = steamUser;
            if (steamUser == null)
                flpOthersInv.Controls.Clear();
            else
                LoadInventory(pnlOther, flpOthersInv, steamUser);
        }

        protected void SetSteamUser(SteamUser steamUser)
        {
            _steamUser = steamUser;
            if (steamUser == null)
                flpMyInv.Controls.Clear();
            else
                LoadInventory(pnlMy, flpMyInv, steamUser);
        }

        private void bwLoadInventory_DoWork(object sender, DoWorkEventArgs e)
        {
            var data = (LoadWorkerData)e.Argument;

            if (data.steamUser.Cards == null)
            {
                string errorMessage;
                data.steamUser.Cards = SteamDataDownloader.GetCards(data.steamUser, out errorMessage);
                data.errorMessage = errorMessage;
            }

            e.Result = data;
        }

        private void bwLoadInventory_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var data = e.Result as LoadWorkerData;
            if (data == null)
                return;

            data.pnl.Enabled = true;
            data.pnl.BackColor = Color.FromKnownColor(KnownColor.ActiveCaption);
            data.flpInv.Enabled = true;

            if (data.errorMessage != null)
            {
                var label = new Label();
                label.Text = data.errorMessage;
                data.flpInv.Controls.Add(label);
            }
        }

        private void LoadInventory(Panel pnl, FlowLayoutPanel flpInv, SteamUser steamUser)
        {
            pnl.Enabled = false;
            pnl.BackColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            flpInv.Controls.Clear();
            flpInv.Enabled = false;
            var data = new LoadWorkerData() { pnl = pnl, flpInv = flpInv, steamUser = steamUser };
            bwLoadInventory.RunWorkerAsync(data);
        }

        private class LoadWorkerData
        {
            public string errorMessage;
            public FlowLayoutPanel flpInv;
            public Panel pnl;
            public SteamUser steamUser;
        }
    }
}