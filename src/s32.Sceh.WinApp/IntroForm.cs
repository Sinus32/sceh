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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var idOrUrl = tbLogin.Text.Trim();

            if (String.IsNullOrEmpty(idOrUrl))
            {
                MessageBox.Show(this, "Please, fill the login first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var data = new LoadAndOpenData() { idOrUrl = idOrUrl };

            btnLogin.Enabled = false;
            bwOpenProfile.RunWorkerAsync(data);
        }

        private void bwOpenProfile_DoWork(object sender, DoWorkEventArgs e)
        {
            var data = (LoadAndOpenData)e.Argument;

            SteamUser steamUser;
            string errorMessage;
            if (SteamUserRepository.Instance.TryGetUser(data.idOrUrl, out steamUser, out errorMessage))
            {
                data.steamUser = steamUser;
            }
            else
            {
                data.errorMessage = errorMessage;
            }

            e.Result = data;
        }

        private void bwOpenProfile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnLogin.Enabled = true;
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
            public string errorMessage;
            public string idOrUrl;
            public SteamUser steamUser;
        }
    }
}