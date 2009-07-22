namespace TradeLink.AppKit
{
    partial class AssemblaTicketWindow
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
            this.assemblaTicketControl1 = new TradeLink.AppKit.AssemblaTicketControl();
            this.SuspendLayout();
            // 
            // assemblaTicketControl1
            // 
            this.assemblaTicketControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assemblaTicketControl1.Location = new System.Drawing.Point(0, 0);
            this.assemblaTicketControl1.Name = "assemblaTicketControl1";
            this.assemblaTicketControl1.Size = new System.Drawing.Size(444, 282);
            this.assemblaTicketControl1.TabIndex = 0;
            // 
            // AssemblaTicketWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 282);
            this.Controls.Add(this.assemblaTicketControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "AssemblaTicketWindow";
            this.Text = "Create ticket for a bug";
            this.ResumeLayout(false);

        }

        #endregion

        private AssemblaTicketControl assemblaTicketControl1;


    }
}