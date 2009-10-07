namespace TradeLink.AppKit
{
    partial class BookControl
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
            this._stat = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _stat
            // 
            this._stat.AutoSize = true;
            this._stat.Location = new System.Drawing.Point(80, 140);
            this._stat.Name = "_stat";
            this._stat.Size = new System.Drawing.Size(114, 20);
            this._stat.TabIndex = 0;
            this._stat.Text = "Enter a symbol";
            this._stat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BookControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._stat);
            this.Name = "BookControl";
            this.Size = new System.Drawing.Size(197, 160);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _stat;
    }
}
