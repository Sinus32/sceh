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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlMy = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlOther = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.flpMyInv = new System.Windows.Forms.FlowLayoutPanel();
            this.flpOthersInv = new System.Windows.Forms.FlowLayoutPanel();
            this.bwLoadInventory = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlMy.SuspendLayout();
            this.pnlOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pnlMy, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlOther, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flpMyInv, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flpOthersInv, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(509, 329);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pnlMy
            // 
            this.pnlMy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMy.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlMy.Controls.Add(this.label1);
            this.pnlMy.Location = new System.Drawing.Point(3, 3);
            this.pnlMy.Name = "pnlMy";
            this.pnlMy.Padding = new System.Windows.Forms.Padding(3);
            this.pnlMy.Size = new System.Drawing.Size(248, 34);
            this.pnlMy.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "My inventory";
            // 
            // pnlOther
            // 
            this.pnlOther.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOther.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlOther.Controls.Add(this.label2);
            this.pnlOther.Location = new System.Drawing.Point(257, 3);
            this.pnlOther.Name = "pnlOther";
            this.pnlOther.Padding = new System.Windows.Forms.Padding(3);
            this.pnlOther.Size = new System.Drawing.Size(249, 34);
            this.pnlOther.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Other\'s inventory";
            // 
            // flpMyInv
            // 
            this.flpMyInv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpMyInv.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpMyInv.Location = new System.Drawing.Point(3, 43);
            this.flpMyInv.Name = "flpMyInv";
            this.flpMyInv.Size = new System.Drawing.Size(248, 283);
            this.flpMyInv.TabIndex = 2;
            // 
            // flpOthersInv
            // 
            this.flpOthersInv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpOthersInv.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpOthersInv.Location = new System.Drawing.Point(257, 43);
            this.flpOthersInv.Name = "flpOthersInv";
            this.flpOthersInv.Size = new System.Drawing.Size(249, 283);
            this.flpOthersInv.TabIndex = 3;
            // 
            // bwLoadInventory
            // 
            this.bwLoadInventory.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwLoadInventory_DoWork);
            this.bwLoadInventory.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwLoadInventory_RunWorkerCompleted);
            // 
            // InvCompareCtr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "InvCompareCtr";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(519, 339);
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
        private System.Windows.Forms.FlowLayoutPanel flpMyInv;
        private System.Windows.Forms.FlowLayoutPanel flpOthersInv;
        private System.ComponentModel.BackgroundWorker bwLoadInventory;


    }
}
