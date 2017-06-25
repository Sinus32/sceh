namespace s32.Sceh.WinApp.Controls
{
    partial class ProfileViewCtr
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lblProfile = new System.Windows.Forms.Label();
            this.btnProfile = new System.Windows.Forms.Button();
            this.btnBadges = new System.Windows.Forms.Button();
            this.Inventory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Profile:";
            // 
            // lblProfile
            // 
            this.lblProfile.AutoSize = true;
            this.lblProfile.Location = new System.Drawing.Point(70, 8);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(46, 13);
            this.lblProfile.TabIndex = 1;
            this.lblProfile.Text = "lblProfile";
            // 
            // btnProfile
            // 
            this.btnProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfile.Location = new System.Drawing.Point(278, 3);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(75, 23);
            this.btnProfile.TabIndex = 2;
            this.btnProfile.Text = "Profile";
            this.btnProfile.UseVisualStyleBackColor = true;
            // 
            // btnBadges
            // 
            this.btnBadges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBadges.Location = new System.Drawing.Point(359, 3);
            this.btnBadges.Name = "btnBadges";
            this.btnBadges.Size = new System.Drawing.Size(75, 23);
            this.btnBadges.TabIndex = 3;
            this.btnBadges.Text = "Badges";
            this.btnBadges.UseVisualStyleBackColor = true;
            // 
            // Inventory
            // 
            this.Inventory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Inventory.Location = new System.Drawing.Point(440, 3);
            this.Inventory.Name = "Inventory";
            this.Inventory.Size = new System.Drawing.Size(75, 23);
            this.Inventory.TabIndex = 4;
            this.Inventory.Text = "Inventory";
            this.Inventory.UseVisualStyleBackColor = true;
            // 
            // ProfileViewCtr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Inventory);
            this.Controls.Add(this.btnBadges);
            this.Controls.Add(this.btnProfile);
            this.Controls.Add(this.lblProfile);
            this.Controls.Add(this.label1);
            this.Name = "ProfileViewCtr";
            this.Size = new System.Drawing.Size(518, 175);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.Button btnBadges;
        private System.Windows.Forms.Button Inventory;
    }
}
