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
using s32.Sceh.Code;

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

        protected void SetSteamUser(SteamUser steamUser)
        {
            _steamUser = steamUser;

            if (steamUser == null)
            {
                lblProfile.Text = String.Empty;
                btnProfile.Visible = false;
                btnBadges.Visible = false;
                btnInventory.Visible = false;
            }
            else
            {
                var name = steamUser.GetName();
                if (name == null)
                    lblProfile.Text = steamUser.SteamId.ToString();
                else
                    lblProfile.Text = String.Format("{0} ({1})", name, steamUser.SteamId);

                btnProfile.Visible = true;
                btnBadges.Visible = true;
                btnInventory.Visible = true;
            }
        }

        private void btnBadges_Click(object sender, EventArgs e)
        {
            var profileUrl = _steamUser.GetProfileUrl(ProfilePage.BADGES);
            if (profileUrl != null)
                Process.Start(profileUrl);
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            var profileUrl = _steamUser.GetProfileUrl(ProfilePage.INVENTORY);
            if (profileUrl != null)
                Process.Start(profileUrl);
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            var profileUrl = _steamUser.GetProfileUrl();
            if (profileUrl != null)
                Process.Start(profileUrl);
        }
    }
}