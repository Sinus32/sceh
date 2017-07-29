namespace s32.Sceh.WinApp.Controls
{
    partial class SteamAppCtl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SteamAppCtl));
            this.appHeader = new s32.Sceh.WinApp.Controls.SteamAppHeader();
            this.SuspendLayout();
            // 
            // appHeader
            // 
            this.appHeader.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.appHeader, "appHeader");
            this.appHeader.Name = "appHeader";
            // 
            // SteamAppCtl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.appHeader);
            this.Name = "SteamAppCtl";
            this.ResumeLayout(false);

        }

        #endregion

        private SteamAppHeader appHeader;

    }
}
