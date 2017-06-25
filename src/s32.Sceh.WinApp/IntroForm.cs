using s32.Sceh.Classes;
using s32.Sceh.WinApp.Code;
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

            SteamUser user;
            string errorMessage;

            if (SteamUserRepository.Instance.TryGetUser(idOrUrl, out user, out errorMessage))
            {
                MessageBox.Show(this, user.Name, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
