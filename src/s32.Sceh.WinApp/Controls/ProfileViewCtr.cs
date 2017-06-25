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
using System.Diagnostics;

namespace s32.Sceh.WinApp.Controls
{
    public partial class ProfileViewCtr : UserControl
    {
        private SteamUser _steamUser;

        public ProfileViewCtr()
        {
            InitializeComponent();
            SetSteamUser(null);
        }

        public SteamUser SteamUser
        {
            get { return _steamUser; }
            set { SetSteamUser(value); }
        }

        protected void SetSteamUser(SteamUser user)
        {
            _steamUser = user;

            if (user == null)
            {
                lblProfile.Text = String.Empty;
                btnProfile.Visible = false;
                btnBadges.Visible = false;
                btnInventory.Visible = false;
            }
            else
            {
                var name = user.GetName();
                if (name == null)
                    lblProfile.Text = user.SteamId.ToString();
                else
                    lblProfile.Text = String.Format("{0} ({1})", name, user.SteamId);

                btnProfile.Visible = true;
                btnBadges.Visible = true;
                btnInventory.Visible = true;
            }
        }

        private void btnBadges_Click(object sender, EventArgs e)
        {
            if (_steamUser == null)
                return;

            var profileUrl = _steamUser.GetProfileUrl("badges");
            if (profileUrl != null)
                Process.Start(profileUrl);
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            if (_steamUser == null)
                return;

            var profileUrl = _steamUser.GetProfileUrl("inventory");
            if (profileUrl != null)
                Process.Start(profileUrl);
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            if (_steamUser == null)
                return;

            var profileUrl = _steamUser.GetProfileUrl();
            if (profileUrl != null)
                Process.Start(profileUrl);
        }
    }
}