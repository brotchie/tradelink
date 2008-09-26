namespace Record
{
    partial class RecordMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordMain));
            this.stockslist = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.symbox = new System.Windows.Forms.ToolStripTextBox();
            this.recordbut = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stockslist
            // 
            this.stockslist.BackColor = System.Drawing.SystemColors.Window;
            this.stockslist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stockslist.FormattingEnabled = true;
            this.stockslist.ItemHeight = 20;
            this.stockslist.Location = new System.Drawing.Point(0, 0);
            this.stockslist.MultiColumn = true;
            this.stockslist.Name = "stockslist";
            this.stockslist.Size = new System.Drawing.Size(290, 244);
            this.stockslist.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.symbox,
            this.recordbut});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(290, 28);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // symbox
            // 
            this.symbox.Name = "symbox";
            this.symbox.Size = new System.Drawing.Size(100, 28);
            this.symbox.ToolTipText = "Enter symbol to record";
            this.symbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.symbox_KeyUp);
            // 
            // recordbut
            // 
            this.recordbut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.recordbut.Image = ((System.Drawing.Image)(resources.GetObject("recordbut.Image")));
            this.recordbut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.recordbut.Name = "recordbut";
            this.recordbut.Size = new System.Drawing.Size(66, 25);
            this.recordbut.Text = "Record";
            this.recordbut.ToolTipText = "Enter a symbol and press record";
            this.recordbut.Click += new System.EventHandler(this.recordbut_Click);
            // 
            // RecordMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 262);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.stockslist);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RecordMain";
            this.Text = "Record";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox stockslist;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripTextBox symbox;
        private System.Windows.Forms.ToolStripButton recordbut;
    }
}

