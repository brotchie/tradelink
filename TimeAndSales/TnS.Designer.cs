namespace TimeSales
{
    partial class TnS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TnS));
            this.tsgrid = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.autoresizebut = new System.Windows.Forms.ToolStripButton();
            this.selected = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.tsgrid)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsgrid
            // 
            this.tsgrid.AllowUserToAddRows = false;
            this.tsgrid.AllowUserToDeleteRows = false;
            this.tsgrid.AllowUserToOrderColumns = true;
            this.tsgrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tsgrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tsgrid.Location = new System.Drawing.Point(0, 0);
            this.tsgrid.Margin = new System.Windows.Forms.Padding(4);
            this.tsgrid.Name = "tsgrid";
            this.tsgrid.ReadOnly = true;
            this.tsgrid.RowTemplate.Height = 24;
            this.tsgrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tsgrid.Size = new System.Drawing.Size(827, 322);
            this.tsgrid.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripProgressBar1,
            this.autoresizebut,
            this.selected});
            this.toolStrip1.Location = new System.Drawing.Point(0, 292);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(827, 30);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 27);
            this.toolStripButton1.Text = "Open";
            this.toolStripButton1.ToolTipText = "Open File";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.Maximum = 102;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(80, 27);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.toolStripProgressBar1.ToolTipText = "Progress complete";
            // 
            // autoresizebut
            // 
            this.autoresizebut.CheckOnClick = true;
            this.autoresizebut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.autoresizebut.Image = ((System.Drawing.Image)(resources.GetObject("autoresizebut.Image")));
            this.autoresizebut.ImageTransparentColor = System.Drawing.Color.White;
            this.autoresizebut.Name = "autoresizebut";
            this.autoresizebut.Size = new System.Drawing.Size(23, 27);
            this.autoresizebut.Text = "Autosize";
            this.autoresizebut.ToolTipText = "Auto Resize Columns after Load";
            this.autoresizebut.CheckedChanged += new System.EventHandler(this.autoresizebut_CheckedChanged);
            // 
            // selected
            // 
            this.selected.Name = "selected";
            this.selected.Size = new System.Drawing.Size(234, 27);
            this.selected.Text = "Click \'Open\' to load time and sales.";
            // 
            // TnS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(827, 322);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tsgrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TnS";
            this.Text = "Time & Sales";
            ((System.ComponentModel.ISupportInitialize)(this.tsgrid)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView tsgrid;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripLabel selected;
        private System.Windows.Forms.ToolStripButton autoresizebut;
    }
}

