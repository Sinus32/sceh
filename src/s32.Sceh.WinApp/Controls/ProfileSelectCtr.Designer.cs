namespace s32.Sceh.WinApp.Controls
{
    partial class ProfileSelectCtr
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileSelectCtr));
            this.cmbOther = new s32.Sceh.WinApp.Controls.SteamProfileComboBox();
            this.SuspendLayout();
            // 
            // lblProfile
            // 
            resources.ApplyResources(this.lblProfile, "lblProfile");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            // 
            // cmbOther
            // 
            this.cmbOther.FormattingEnabled = true;
            resources.ApplyResources(this.cmbOther, "cmbOther");
            this.cmbOther.Name = "cmbOther";
            this.cmbOther.SelectedProfile = null;
            // 
            // ProfileSelectCtr
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cmbOther);
            this.Name = "ProfileSelectCtr";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.lblProfile, 0);
            this.Controls.SetChildIndex(this.btnProfile, 0);
            this.Controls.SetChildIndex(this.btnBadges, 0);
            this.Controls.SetChildIndex(this.btnInventory, 0);
            this.Controls.SetChildIndex(this.cmbOther, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SteamProfileComboBox cmbOther;

    }
}
