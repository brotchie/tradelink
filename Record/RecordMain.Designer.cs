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
            this.stockslist.Size = new System.Drawing.Size(326, 244);
            this.stockslist.TabIndex = 0;
            // 
            // RecordMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 244);
            this.Controls.Add(this.stockslist);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RecordMain";
            this.Text = "Record";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox stockslist;
    }
}

