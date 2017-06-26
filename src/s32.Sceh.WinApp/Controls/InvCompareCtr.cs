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
                FillAreaWithSuggestions();
            else
                LoadInventory(pnlOther);
        }

        protected void SetSteamUser(SteamUser steamUser)
        {
            _steamUser = steamUser;
            if (steamUser == null)
                FillAreaWithSuggestions();
            else
                LoadInventory(pnlMy);
        }

        private void FillAreaWithSuggestions()
        {
            flpSteamApps.Controls.Clear();
            var emptyCardList = new List<Card>();
            var myCards = _steamUser == null || _steamUser.Cards == null ? emptyCardList : _steamUser.Cards;
            var otherCards = _otherUser == null || _otherUser.Cards == null ? emptyCardList : _otherUser.Cards;

            string errorMessage;
            var steamApps = TradeSuggestionsMaker.Generate(myCards, otherCards, out errorMessage);

            if (errorMessage != null)
            {
                var label = new Label();
                label.Text = errorMessage;
                flpSteamApps.Controls.Add(label);
            }
            else
            {
                foreach (var app in steamApps)
                {
                    var ctl = new SteamAppCtl();
                    ctl.SteamApp = app;
                    flpSteamApps.Controls.Add(ctl);
                }
            }
        }

        private void LoadInventory(Panel pnl)
        {
            pnl.Enabled = false;
            pnl.BackColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            flpSteamApps.Controls.Clear();
            flpSteamApps.Enabled = false;
            bwLoadMyInventory.RunWorkerAsync(pnl);
        }

        private void bwOtherMyInventory_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_otherUser.Cards == null)
            {
                string errorMessage;
                _otherUser.Cards = SteamDataDownloader.GetCards(_otherUser, out errorMessage);
                e.Result = errorMessage;
            }

            e.Result = null;
        }

        private void bwOtherMyInventory_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pnlOther.Enabled = true;
            pnlOther.BackColor = Color.FromKnownColor(KnownColor.ActiveCaption);
            flpSteamApps.Enabled = true;

            var errorMessage = e.Result as string;
            if (errorMessage != null)
            {
                var label = new Label();
                label.Text = errorMessage;
                flpSteamApps.Controls.Add(label);
            }
            else
            {
                FillAreaWithSuggestions();
            }
        }

        private void bwLoadMyInventory_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_steamUser.Cards == null)
            {
                string errorMessage;
                _steamUser.Cards = SteamDataDownloader.GetCards(_steamUser, out errorMessage);
                e.Result = errorMessage;
            }

            e.Result = null;
        }

        private void bwLoadMyInventory_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pnlMy.Enabled = true;
            pnlMy.BackColor = Color.FromKnownColor(KnownColor.ActiveCaption);
            flpSteamApps.Enabled = true;

            var errorMessage = e.Result as string;
            if (errorMessage != null)
            {
                var label = new Label();
                label.Text = errorMessage;
                flpSteamApps.Controls.Add(label);
            }
            else
            {
                FillAreaWithSuggestions();
            }
        }
    }
}