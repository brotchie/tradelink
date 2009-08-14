namespace CQG2EPF
{
    partial class TikConvertMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TikConvertMain));
            this._msg = new System.Windows.Forms.ListBox();
            this._inputbut = new System.Windows.Forms.Button();
            this._defaultsize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this._progress = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this._defaultsize)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _msg
            // 
            this._msg.FormattingEnabled = true;
            this._msg.ItemHeight = 20;
            this._msg.Location = new System.Drawing.Point(31, 67);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(315, 144);
            this._msg.TabIndex = 0;
            // 
            // _inputbut
            // 
            this._inputbut.Location = new System.Drawing.Point(31, 23);
            this._inputbut.Name = "_inputbut";
            this._inputbut.Size = new System.Drawing.Size(144, 36);
            this._inputbut.TabIndex = 1;
            this._inputbut.Text = "select input files";
            this._inputbut.UseVisualStyleBackColor = true;
            this._inputbut.Click += new System.EventHandler(this._inputbut_Click);
            // 
            // _defaultsize
            // 
            this._defaultsize.Location = new System.Drawing.Point(237, 35);
            this._defaultsize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._defaultsize.Name = "_defaultsize";
            this._defaultsize.Size = new System.Drawing.Size(82, 26);
            this._defaultsize.TabIndex = 2;
            this._defaultsize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._defaultsize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(209, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Default TradeSize";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._progress});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 226);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(376, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // _progress
            // 
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(325, 16);
            // 
            // CQG2EPFMAIN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 248);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._defaultsize);
            this.Controls.Add(this._inputbut);
            this.Controls.Add(this._msg);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CQG2EPFMAIN";
            this.Text = "CQG2EPF";
            ((System.ComponentModel.ISupportInitialize)(this._defaultsize)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox _msg;
        private System.Windows.Forms.Button _inputbut;
        private System.Windows.Forms.NumericUpDown _defaultsize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar _progress;
    }
}

