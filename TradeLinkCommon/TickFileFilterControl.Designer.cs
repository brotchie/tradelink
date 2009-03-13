namespace TradeLink.Common
{
    partial class TickFileFilterControl
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
            this.usedates = new System.Windows.Forms.CheckBox();
            this.usestocks = new System.Windows.Forms.CheckBox();
            this.daylist = new System.Windows.Forms.ListBox();
            this.monthlist = new System.Windows.Forms.ListBox();
            this.yearlist = new System.Windows.Forms.ListBox();
            this.stocklist = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // usedates
            // 
            this.usedates.Appearance = System.Windows.Forms.Appearance.Button;
            this.usedates.AutoSize = true;
            this.usedates.Location = new System.Drawing.Point(4, 160);
            this.usedates.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.usedates.Name = "usedates";
            this.usedates.Size = new System.Drawing.Size(62, 30);
            this.usedates.TabIndex = 19;
            this.usedates.Text = "Dates";
            this.usedates.UseVisualStyleBackColor = true;
            this.usedates.CheckedChanged += new System.EventHandler(this.usedates_CheckedChanged);
            // 
            // usestocks
            // 
            this.usestocks.Appearance = System.Windows.Forms.Appearance.Button;
            this.usestocks.AutoSize = true;
            this.usestocks.Location = new System.Drawing.Point(4, 5);
            this.usestocks.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.usestocks.Name = "usestocks";
            this.usestocks.Size = new System.Drawing.Size(79, 30);
            this.usestocks.TabIndex = 18;
            this.usestocks.Text = "Symbols";
            this.usestocks.UseVisualStyleBackColor = true;
            this.usestocks.CheckedChanged += new System.EventHandler(this.usestocks_CheckedChanged);
            // 
            // daylist
            // 
            this.daylist.ColumnWidth = 30;
            this.daylist.Enabled = false;
            this.daylist.HorizontalScrollbar = true;
            this.daylist.ItemHeight = 20;
            this.daylist.Location = new System.Drawing.Point(230, 160);
            this.daylist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.daylist.MultiColumn = true;
            this.daylist.Name = "daylist";
            this.daylist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.daylist.Size = new System.Drawing.Size(99, 124);
            this.daylist.TabIndex = 17;
            this.daylist.SelectedIndexChanged += new System.EventHandler(this.daylist_SelectedIndexChanged);
            // 
            // monthlist
            // 
            this.monthlist.Enabled = false;
            this.monthlist.FormattingEnabled = true;
            this.monthlist.ItemHeight = 20;
            this.monthlist.Location = new System.Drawing.Point(168, 160);
            this.monthlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.monthlist.Name = "monthlist";
            this.monthlist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.monthlist.Size = new System.Drawing.Size(54, 124);
            this.monthlist.TabIndex = 16;
            this.monthlist.SelectedIndexChanged += new System.EventHandler(this.monthlist_SelectedIndexChanged);
            // 
            // yearlist
            // 
            this.yearlist.Enabled = false;
            this.yearlist.FormattingEnabled = true;
            this.yearlist.ItemHeight = 20;
            this.yearlist.Location = new System.Drawing.Point(96, 161);
            this.yearlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.yearlist.Name = "yearlist";
            this.yearlist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.yearlist.Size = new System.Drawing.Size(62, 124);
            this.yearlist.Sorted = true;
            this.yearlist.TabIndex = 15;
            this.yearlist.SelectedIndexChanged += new System.EventHandler(this.yearlist_SelectedIndexChanged);
            // 
            // stocklist
            // 
            this.stocklist.Enabled = false;
            this.stocklist.FormattingEnabled = true;
            this.stocklist.ItemHeight = 20;
            this.stocklist.Location = new System.Drawing.Point(96, 4);
            this.stocklist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stocklist.MultiColumn = true;
            this.stocklist.Name = "stocklist";
            this.stocklist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.stocklist.Size = new System.Drawing.Size(506, 124);
            this.stocklist.TabIndex = 14;
            this.stocklist.SelectedIndexChanged += new System.EventHandler(this.stocklist_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(226, 136);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 20);
            this.label6.TabIndex = 22;
            this.label6.Text = "Day";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(164, 136);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 20);
            this.label5.TabIndex = 21;
            this.label5.Text = "Month";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(92, 136);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 20);
            this.label3.TabIndex = 20;
            this.label3.Text = "Year";
            // 
            // TickFileFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.usedates);
            this.Controls.Add(this.usestocks);
            this.Controls.Add(this.daylist);
            this.Controls.Add(this.monthlist);
            this.Controls.Add(this.yearlist);
            this.Controls.Add(this.stocklist);
            this.Name = "TickFileFilterControl";
            this.Size = new System.Drawing.Size(607, 298);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox usedates;
        private System.Windows.Forms.CheckBox usestocks;
        private System.Windows.Forms.ListBox daylist;
        private System.Windows.Forms.ListBox monthlist;
        private System.Windows.Forms.ListBox yearlist;
        private System.Windows.Forms.ListBox stocklist;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;

    }
}
