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
        private SteamUser _steamUser;
        private SteamUser _otherUser;

        public InvCompareCtr()
        {
            InitializeComponent();
        }

        public SteamUser SteamUser
        {
            get { return _steamUser; }
            set { SetSteamUser(value); }
        }

        protected void SetSteamUser(SteamUser steamUser)
        {
            _steamUser = steamUser;
            if (steamUser == null)
                flpMyInv.Controls.Clear();
            else
                LoadInventory(flpMyInv, steamUser);
        }

        protected void SetOtherUser(SteamUser steamUser)
        {
            _otherUser = steamUser;
            if (steamUser == null)
                flpOthersInv.Controls.Clear();
            else
                LoadInventory(flpOthersInv, steamUser);
        }

        private void LoadInventory(FlowLayoutPanel flpInv, SteamUser steamUser)
        {
            flpMyInv.Controls.Clear();

            if (steamUser.Cards == null)
            {
                string errorMessage;
                steamUser.Cards = SteamDataDownloader.GetCards(steamUser, out errorMessage);

                if (errorMessage != null)
                {
                    var label = new Label();
                    label.Text = errorMessage;
                    flpInv.Controls.Add(label);
                }
            }
        }
    }
}
