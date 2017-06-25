using s32.Sceh.Classes;
using s32.Sceh.WinApp.Code;
using s32.Sceh.WinApp.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace s32.Sceh.WinApp
{
    public partial class IntroForm : Form
    {
        public IntroForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            //76561198353206398

            //cmbLogin.Items.Add

            var profiles = new SteamProfile[]
            {
                new SteamProfile() {CustomURL = "sinus32", Name = "Sinus32"},
                new SteamProfile() {SteamId = 76561198353206398, Name = "Ravera_6"},
            };

            foreach (var dt in profiles)
                cmbLogin.AddItem(dt);

            base.OnLoad(e);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            LoadAndOpenData data;

            var item = cmbLogin.SelectedProfile;
            if (item != null)
            {
                data = new LoadAndOpenData() { customURL = item.CustomURL, steamId = item.SteamId };
            }
            else
            {
                var idOrUrl = cmbLogin.Text.Trim();

                if (String.IsNullOrEmpty(idOrUrl))
                {
                    MessageBox.Show(this, "Please, fill the login first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                data = new LoadAndOpenData() { idOrUrl = idOrUrl };
            }

            btnLogin.Enabled = false;
            bwOpenProfile.RunWorkerAsync(data);
        }

        private void bwOpenProfile_DoWork(object sender, DoWorkEventArgs e)
        {
            var data = (LoadAndOpenData)e.Argument;

            SteamUser steamUser;
            string errorMessage;
            if (data.steamId > 0 || !String.IsNullOrEmpty(data.customURL))
            {
                if (SteamUserRepository.Instance.TryGetUser(data.steamId, data.customURL, out steamUser, out errorMessage))
                {
                    data.steamUser = steamUser;
                }
                else
                {
                    data.errorMessage = errorMessage;
                }
            }
            else
            {
                if (SteamUserRepository.Instance.TryGetUser(data.idOrUrl, out steamUser, out errorMessage))
                {
                    data.steamUser = steamUser;
                }
                else
                {
                    data.errorMessage = errorMessage;
                }
            }
            e.Result = data;
        }

        private void bwOpenProfile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var data = e.Result as LoadAndOpenData;

            if (data == null)
            {
                MessageBox.Show(this, "Failed to load profile", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (data.errorMessage != null)
            {
                MessageBox.Show(this, data.errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (data.steamUser != null)
                OpenInvCompare(data.steamUser);
            btnLogin.Enabled = true;
        }

        private void OpenInvCompare(SteamUser user)
        {
            var invCompareForm = new InvCompareForm();
            invCompareForm.SetSteamUser(user);
            this.Hide();
            var result = invCompareForm.ShowDialog(this);
            if (result == DialogResult.Yes)
                this.Show();
            else
                this.Close();
        }

        private class LoadAndOpenData
        {
            public string customURL;
            public string errorMessage;
            public string idOrUrl;
            public long steamId;
            public SteamUser steamUser;
        }
    }
}