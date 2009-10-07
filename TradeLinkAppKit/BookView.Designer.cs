namespace TradeLink.AppKit
{
    partial class BookView
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
            this.bookControl1 = new TradeLink.AppKit.BookControl();
            this.SuspendLayout();
            // 
            // bookControl1
            // 
            this.bookControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookControl1.Location = new System.Drawing.Point(0, 0);
            this.bookControl1.Name = "bookControl1";
            this.bookControl1.Size = new System.Drawing.Size(605, 266);
            this.bookControl1.Symbol = "";
            this.bookControl1.TabIndex = 0;
            // 
            // BookView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 266);
            this.Controls.Add(this.bookControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "BookView";
            this.Text = "BookView";
            this.ResumeLayout(false);

        }

        #endregion

        private BookControl bookControl1;
    }
}