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
using System.Runtime.InteropServices;

namespace s32.Sceh.WinApp.Controls
{
    public partial class InvCompareCtr : UserControl
    {
        private SteamUser _otherUser;
        private SteamUser _steamUser;

        public InvCompareCtr()
        {
            InitializeComponent();
            pnlSteamApps.HorizontalScroll.Visible = false;
        }

        public SteamUser SteamUser
        {
            get { return _steamUser; }
            set { SetSteamUser(value); }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(HandleRef hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        private static void EnableRepaint(HandleRef handle, bool enable)
        {
            const int WM_SETREDRAW = 0x000B;
            SendMessage(handle, WM_SETREDRAW, new IntPtr(enable ? 1 : 0), IntPtr.Zero);
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
            pnlSteamApps.Controls.Clear();
            var emptyCardList = new List<Card>();
            var myCards = _steamUser == null || _steamUser.Cards == null ? emptyCardList : _steamUser.Cards;
            var otherCards = _otherUser == null || _otherUser.Cards == null ? emptyCardList : _otherUser.Cards;

            string errorMessage;
            var steamApps = TradeSuggestionsMaker.Generate(myCards, otherCards, out errorMessage);

            if (errorMessage != null)
            {
                var label = new Label();
                label.Text = errorMessage;
                pnlSteamApps.Controls.Add(label);
            }
            else
            {
                this.SuspendLayout();
                HandleRef hnd = new HandleRef(this, this.Handle);
                EnableRepaint(hnd, false);
                try
                {
                    foreach (var app in steamApps)
                    {
                        var ctl = new SteamAppCtl();
                        ctl.SteamApp = app;
                        ctl.Dock = DockStyle.Top;
                        pnlSteamApps.Controls.Add(ctl);
                    }
                }
                finally
                {
                    EnableRepaint(hnd, true);
                    this.ResumeLayout();

                    this.Refresh();
                }
            }
        }

        private void LoadInventory(Panel pnl)
        {
            pnl.Enabled = false;
            pnl.BackColor = Color.FromKnownColor(KnownColor.InactiveCaption);
            pnlSteamApps.Controls.Clear();
            pnlSteamApps.Enabled = false;
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
            pnlSteamApps.Enabled = true;

            var errorMessage = e.Result as string;
            if (errorMessage != null)
            {
                var label = new Label();
                label.Text = errorMessage;
                pnlSteamApps.Controls.Add(label);
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
            pnlSteamApps.Enabled = true;

            var errorMessage = e.Result as string;
            if (errorMessage != null)
            {
                var label = new Label();
                label.Text = errorMessage;
                pnlSteamApps.Controls.Add(label);
            }
            else
            {
                FillAreaWithSuggestions();
            }
        }

        private void vsbScroll_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == e.NewValue)
                return;

            pnlSteamApps.AutoScrollPosition = new Point(0, e.NewValue);
        }

        private void pnlSteamApps_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}