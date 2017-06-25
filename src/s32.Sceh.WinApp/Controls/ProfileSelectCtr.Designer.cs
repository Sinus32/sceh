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
            this.cmbProfile = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblProfile
            // 
            this.lblProfile.Visible = false;
            // 
            // label1
            // 
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.Text = "Other profile:";
            // 
            // cmbProfile
            // 
            this.cmbProfile.FormattingEnabled = true;
            this.cmbProfile.Location = new System.Drawing.Point(80, 5);
            this.cmbProfile.Name = "cmbProfile";
            this.cmbProfile.Size = new System.Drawing.Size(200, 21);
            this.cmbProfile.TabIndex = 5;
            // 
            // ProfileSelectCtr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.cmbProfile);
            this.Name = "ProfileSelectCtr";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.lblProfile, 0);
            this.Controls.SetChildIndex(this.btnProfile, 0);
            this.Controls.SetChildIndex(this.btnBadges, 0);
            this.Controls.SetChildIndex(this.btnInventory, 0);
            this.Controls.SetChildIndex(this.cmbProfile, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbProfile;
    }
}
