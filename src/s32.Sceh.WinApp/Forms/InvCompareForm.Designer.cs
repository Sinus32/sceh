namespace s32.Sceh.WinApp.Forms
{
    partial class InvCompareForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.icInventories = new s32.Sceh.WinApp.Controls.InvCompareCtr();
            this.psOther = new s32.Sceh.WinApp.Controls.ProfileSelectCtr();
            this.pvCurrent = new s32.Sceh.WinApp.Controls.ProfileViewCtr();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(731, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeUserToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // changeUserToolStripMenuItem
            // 
            this.changeUserToolStripMenuItem.Name = "changeUserToolStripMenuItem";
            this.changeUserToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.changeUserToolStripMenuItem.Text = "&Change user";
            this.changeUserToolStripMenuItem.Click += new System.EventHandler(this.changeUserToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 509);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(731, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // icInventories
            // 
            this.icInventories.BackColor = System.Drawing.SystemColors.Window;
            this.icInventories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.icInventories.Location = new System.Drawing.Point(0, 84);
            this.icInventories.Name = "icInventories";
            this.icInventories.Padding = new System.Windows.Forms.Padding(5);
            this.icInventories.Size = new System.Drawing.Size(731, 425);
            this.icInventories.TabIndex = 4;
            // 
            // psOther
            // 
            this.psOther.Dock = System.Windows.Forms.DockStyle.Top;
            this.psOther.Location = new System.Drawing.Point(0, 54);
            this.psOther.Name = "psOther";
            this.psOther.Size = new System.Drawing.Size(731, 30);
            this.psOther.TabIndex = 3;
            // 
            // pvCurrent
            // 
            this.pvCurrent.Dock = System.Windows.Forms.DockStyle.Top;
            this.pvCurrent.Location = new System.Drawing.Point(0, 24);
            this.pvCurrent.Name = "pvCurrent";
            this.pvCurrent.Size = new System.Drawing.Size(731, 30);
            this.pvCurrent.TabIndex = 2;
            // 
            // InvCompareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 531);
            this.Controls.Add(this.icInventories);
            this.Controls.Add(this.psOther);
            this.Controls.Add(this.pvCurrent);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "InvCompareForm";
            this.Text = "InvCompareForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private Controls.ProfileViewCtr pvCurrent;
        private Controls.ProfileSelectCtr psOther;
        private Controls.InvCompareCtr icInventories;
    }
}