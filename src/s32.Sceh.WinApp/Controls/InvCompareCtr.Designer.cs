namespace s32.Sceh.WinApp.Controls
{
    partial class InvCompareCtr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InvCompareCtr));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlMy = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlOther = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.flpSteamApps = new System.Windows.Forms.FlowLayoutPanel();
            this.bwLoadMyInventory = new System.ComponentModel.BackgroundWorker();
            this.bwOtherMyInventory = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlMy.SuspendLayout();
            this.pnlOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pnlMy, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlOther, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flpSteamApps, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pnlMy
            // 
            resources.ApplyResources(this.pnlMy, "pnlMy");
            this.pnlMy.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlMy.Controls.Add(this.label1);
            this.pnlMy.Name = "pnlMy";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // pnlOther
            // 
            resources.ApplyResources(this.pnlOther, "pnlOther");
            this.pnlOther.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlOther.Controls.Add(this.label2);
            this.pnlOther.Name = "pnlOther";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // flpSteamApps
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flpSteamApps, 2);
            resources.ApplyResources(this.flpSteamApps, "flpSteamApps");
            this.flpSteamApps.Name = "flpSteamApps";
            // 
            // bwLoadMyInventory
            // 
            this.bwLoadMyInventory.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwLoadMyInventory_DoWork);
            this.bwLoadMyInventory.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwLoadMyInventory_RunWorkerCompleted);
            // 
            // bwOtherMyInventory
            // 
            this.bwOtherMyInventory.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwOtherMyInventory_DoWork);
            this.bwOtherMyInventory.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwOtherMyInventory_RunWorkerCompleted);
            // 
            // InvCompareCtr
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "InvCompareCtr";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlMy.ResumeLayout(false);
            this.pnlMy.PerformLayout();
            this.pnlOther.ResumeLayout(false);
            this.pnlOther.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnlMy;
        private System.Windows.Forms.Panel pnlOther;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flpSteamApps;
        private System.ComponentModel.BackgroundWorker bwLoadMyInventory;
        private System.ComponentModel.BackgroundWorker bwOtherMyInventory;


    }
}
