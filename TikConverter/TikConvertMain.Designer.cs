namespace TikConverter
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
            this._con = new System.Windows.Forms.ComboBox();
            this._conlab = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cqgparseoptionsgroupbox = new System.Windows.Forms.GroupBox();
            this._decimalplaceslabel = new System.Windows.Forms.Label();
            this._cqgdecimalplacesinput = new System.Windows.Forms.NumericUpDown();
            this._conversionstatusgroupbox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this._defaultsize)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this._cqgparseoptionsgroupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cqgdecimalplacesinput)).BeginInit();
            this._conversionstatusgroupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _msg
            // 
            this._msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this._msg.FormattingEnabled = true;
            this._msg.HorizontalScrollbar = true;
            this._msg.Location = new System.Drawing.Point(3, 16);
            this._msg.Margin = new System.Windows.Forms.Padding(2);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(314, 290);
            this._msg.TabIndex = 0;
            // 
            // _inputbut
            // 
            this._inputbut.Location = new System.Drawing.Point(21, 15);
            this._inputbut.Margin = new System.Windows.Forms.Padding(2);
            this._inputbut.Name = "_inputbut";
            this._inputbut.Size = new System.Drawing.Size(96, 23);
            this._inputbut.TabIndex = 1;
            this._inputbut.Text = "convert";
            this._inputbut.UseVisualStyleBackColor = true;
            this._inputbut.Click += new System.EventHandler(this._inputbut_Click);
            // 
            // _defaultsize
            // 
            this._defaultsize.Location = new System.Drawing.Point(242, 21);
            this._defaultsize.Margin = new System.Windows.Forms.Padding(2);
            this._defaultsize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._defaultsize.Name = "_defaultsize";
            this._defaultsize.Size = new System.Drawing.Size(55, 20);
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
            this.label1.Location = new System.Drawing.Point(220, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Default TradeSize";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._progress});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 441);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 9, 0);
            this.statusStrip1.Size = new System.Drawing.Size(367, 16);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // _progress
            // 
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(280, 10);
            // 
            // _con
            // 
            this._con.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this._con.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this._con.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._con.FormattingEnabled = true;
            this._con.Location = new System.Drawing.Point(130, 18);
            this._con.Margin = new System.Windows.Forms.Padding(2);
            this._con.Name = "_con";
            this._con.Size = new System.Drawing.Size(102, 21);
            this._con.TabIndex = 5;
            this._con.SelectedIndexChanged += new System.EventHandler(this._con_SelectedIndexChanged);
            // 
            // _conlab
            // 
            this._conlab.AutoSize = true;
            this._conlab.Location = new System.Drawing.Point(154, 6);
            this._conlab.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._conlab.Name = "_conlab";
            this._conlab.Size = new System.Drawing.Size(41, 13);
            this._conlab.TabIndex = 6;
            this._conlab.Text = "Source";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this._cqgparseoptionsgroupbox);
            this.flowLayoutPanel1.Controls.Add(this._conversionstatusgroupbox);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 58);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(343, 380);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // _cqgparseoptionsgroupbox
            // 
            this._cqgparseoptionsgroupbox.Controls.Add(this._decimalplaceslabel);
            this._cqgparseoptionsgroupbox.Controls.Add(this._cqgdecimalplacesinput);
            this._cqgparseoptionsgroupbox.Location = new System.Drawing.Point(3, 3);
            this._cqgparseoptionsgroupbox.Name = "_cqgparseoptionsgroupbox";
            this._cqgparseoptionsgroupbox.Size = new System.Drawing.Size(320, 54);
            this._cqgparseoptionsgroupbox.TabIndex = 8;
            this._cqgparseoptionsgroupbox.TabStop = false;
            this._cqgparseoptionsgroupbox.Text = "CQG Parse Options";
            // 
            // _decimalplaceslabel
            // 
            this._decimalplaceslabel.AutoSize = true;
            this._decimalplaceslabel.Location = new System.Drawing.Point(6, 26);
            this._decimalplaceslabel.Name = "_decimalplaceslabel";
            this._decimalplaceslabel.Size = new System.Drawing.Size(80, 13);
            this._decimalplaceslabel.TabIndex = 2;
            this._decimalplaceslabel.Text = "Decimal Places";
            // 
            // _cqgdecimalplacesinput
            // 
            this._cqgdecimalplacesinput.Location = new System.Drawing.Point(92, 24);
            this._cqgdecimalplacesinput.Name = "_cqgdecimalplacesinput";
            this._cqgdecimalplacesinput.Size = new System.Drawing.Size(120, 20);
            this._cqgdecimalplacesinput.TabIndex = 1;
            this._cqgdecimalplacesinput.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // _conversionstatusgroupbox
            // 
            this._conversionstatusgroupbox.Controls.Add(this._msg);
            this._conversionstatusgroupbox.Location = new System.Drawing.Point(3, 63);
            this._conversionstatusgroupbox.Name = "_conversionstatusgroupbox";
            this._conversionstatusgroupbox.Size = new System.Drawing.Size(320, 310);
            this._conversionstatusgroupbox.TabIndex = 9;
            this._conversionstatusgroupbox.TabStop = false;
            this._conversionstatusgroupbox.Text = "Conversion Status";
            // 
            // TikConvertMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 457);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this._con);
            this.Controls.Add(this._conlab);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._defaultsize);
            this.Controls.Add(this._inputbut);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TikConvertMain";
            this.Text = "TikConverter";
            this.Load += new System.EventHandler(this.TikConvertMain_Load);
            this.SizeChanged += new System.EventHandler(this.TikConvertMain_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this._defaultsize)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this._cqgparseoptionsgroupbox.ResumeLayout(false);
            this._cqgparseoptionsgroupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cqgdecimalplacesinput)).EndInit();
            this._conversionstatusgroupbox.ResumeLayout(false);
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
        private System.Windows.Forms.ComboBox _con;
        private System.Windows.Forms.Label _conlab;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.NumericUpDown _cqgdecimalplacesinput;
        private System.Windows.Forms.GroupBox _cqgparseoptionsgroupbox;
        private System.Windows.Forms.Label _decimalplaceslabel;
        private System.Windows.Forms.GroupBox _conversionstatusgroupbox;
    }
}

