namespace ASP
{
    partial class ASP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ASP));
            this.LoadDLL = new System.Windows.Forms.Button();
            this.Boxes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Trade = new System.Windows.Forms.Button();
            this.boxcriteria = new System.Windows.Forms.ListBox();
            this.stock = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.shutdown = new System.Windows.Forms.Button();
            this.activate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.debugon = new System.Windows.Forms.CheckBox();
            this.archivetickbox = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadDLL
            // 
            this.LoadDLL.Location = new System.Drawing.Point(602, 273);
            this.LoadDLL.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LoadDLL.Name = "LoadDLL";
            this.LoadDLL.Size = new System.Drawing.Size(95, 29);
            this.LoadDLL.TabIndex = 0;
            this.LoadDLL.Text = "Box Library";
            this.LoadDLL.UseVisualStyleBackColor = true;
            this.LoadDLL.Click += new System.EventHandler(this.LoadDLL_Click);
            // 
            // Boxes
            // 
            this.Boxes.FormattingEnabled = true;
            this.Boxes.Location = new System.Drawing.Point(377, 9);
            this.Boxes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Boxes.Name = "Boxes";
            this.Boxes.Size = new System.Drawing.Size(320, 28);
            this.Boxes.TabIndex = 1;
            this.Boxes.Text = "Select a box";
            this.Boxes.SelectedIndexChanged += new System.EventHandler(this.Boxes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(245, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Configure Box";
            // 
            // Trade
            // 
            this.Trade.Location = new System.Drawing.Point(585, 53);
            this.Trade.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Trade.Name = "Trade";
            this.Trade.Size = new System.Drawing.Size(112, 35);
            this.Trade.TabIndex = 3;
            this.Trade.Text = "Trade";
            this.Trade.UseVisualStyleBackColor = true;
            this.Trade.Click += new System.EventHandler(this.Trade_Click);
            // 
            // boxcriteria
            // 
            this.boxcriteria.FormattingEnabled = true;
            this.boxcriteria.HorizontalScrollbar = true;
            this.boxcriteria.ItemHeight = 20;
            this.boxcriteria.Location = new System.Drawing.Point(10, 59);
            this.boxcriteria.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.boxcriteria.Name = "boxcriteria";
            this.boxcriteria.Size = new System.Drawing.Size(222, 204);
            this.boxcriteria.TabIndex = 4;
            this.boxcriteria.TabStop = false;
            this.boxcriteria.SelectedIndexChanged += new System.EventHandler(this.boxcriteria_SelectedIndexChanged);
            // 
            // stock
            // 
            this.stock.Location = new System.Drawing.Point(377, 57);
            this.stock.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stock.Name = "stock";
            this.stock.Size = new System.Drawing.Size(196, 26);
            this.stock.TabIndex = 5;
            this.stock.KeyUp += new System.Windows.Forms.KeyEventHandler(this.stock_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(245, 63);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Trade on Symbol";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 326);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(704, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // shutdown
            // 
            this.shutdown.Location = new System.Drawing.Point(10, 273);
            this.shutdown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.shutdown.Name = "shutdown";
            this.shutdown.Size = new System.Drawing.Size(92, 29);
            this.shutdown.TabIndex = 9;
            this.shutdown.TabStop = false;
            this.shutdown.Text = "Shutdown";
            this.shutdown.UseVisualStyleBackColor = true;
            this.shutdown.Click += new System.EventHandler(this.shutdown_Click);
            // 
            // activate
            // 
            this.activate.Location = new System.Drawing.Point(110, 273);
            this.activate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.activate.Name = "activate";
            this.activate.Size = new System.Drawing.Size(96, 29);
            this.activate.TabIndex = 10;
            this.activate.TabStop = false;
            this.activate.Text = "Activate";
            this.activate.UseVisualStyleBackColor = true;
            this.activate.Click += new System.EventHandler(this.activate_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 21);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Configured Boxes:";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(249, 99);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(448, 164);
            this.listBox1.TabIndex = 12;
            // 
            // debugon
            // 
            this.debugon.Appearance = System.Windows.Forms.Appearance.Button;
            this.debugon.AutoSize = true;
            this.debugon.Checked = true;
            this.debugon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.debugon.Location = new System.Drawing.Point(249, 272);
            this.debugon.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.debugon.Name = "debugon";
            this.debugon.Size = new System.Drawing.Size(75, 30);
            this.debugon.TabIndex = 13;
            this.debugon.Text = "Debugs";
            this.debugon.UseVisualStyleBackColor = true;
            this.debugon.CheckedChanged += new System.EventHandler(this.debugon_CheckedChanged);
            // 
            // archivetickbox
            // 
            this.archivetickbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.archivetickbox.AutoSize = true;
            this.archivetickbox.Checked = true;
            this.archivetickbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.archivetickbox.Location = new System.Drawing.Point(335, 272);
            this.archivetickbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.archivetickbox.Name = "archivetickbox";
            this.archivetickbox.Size = new System.Drawing.Size(95, 30);
            this.archivetickbox.TabIndex = 14;
            this.archivetickbox.Text = "Save Ticks";
            this.archivetickbox.UseVisualStyleBackColor = true;
            // 
            // ASP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 348);
            this.Controls.Add(this.archivetickbox);
            this.Controls.Add(this.debugon);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.activate);
            this.Controls.Add(this.shutdown);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.stock);
            this.Controls.Add(this.boxcriteria);
            this.Controls.Add(this.Trade);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Boxes);
            this.Controls.Add(this.LoadDLL);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ASP";
            this.Text = "ASP";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadDLL;
        private System.Windows.Forms.ComboBox Boxes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Trade;
        private System.Windows.Forms.ListBox boxcriteria;
        private System.Windows.Forms.TextBox stock;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button shutdown;
        private System.Windows.Forms.Button activate;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox debugon;
        private System.Windows.Forms.CheckBox archivetickbox;
    }
}

