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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileViewCtr));
            this.label1 = new System.Windows.Forms.Label();
            this.lblProfile = new System.Windows.Forms.Label();
            this.btnProfile = new System.Windows.Forms.Button();
            this.btnBadges = new System.Windows.Forms.Button();
            this.btnInventory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblProfile
            // 
            resources.ApplyResources(this.lblProfile, "lblProfile");
            this.lblProfile.Name = "lblProfile";
            // 
            // btnProfile
            // 
            resources.ApplyResources(this.btnProfile, "btnProfile");
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.UseVisualStyleBackColor = true;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // btnBadges
            // 
            resources.ApplyResources(this.btnBadges, "btnBadges");
            this.btnBadges.Name = "btnBadges";
            this.btnBadges.UseVisualStyleBackColor = true;
            this.btnBadges.Click += new System.EventHandler(this.btnBadges_Click);
            // 
            // btnInventory
            // 
            resources.ApplyResources(this.btnInventory, "btnInventory");
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // ProfileViewCtr
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.btnBadges);
            this.Controls.Add(this.btnProfile);
            this.Controls.Add(this.lblProfile);
            this.Controls.Add(this.label1);
            this.Name = "ProfileViewCtr";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Label lblProfile;
        protected System.Windows.Forms.Button btnProfile;
        protected System.Windows.Forms.Button btnBadges;
        protected System.Windows.Forms.Button btnInventory;
        protected System.Windows.Forms.Label label1;
    }
}
