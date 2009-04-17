namespace TradeLink.Common
{
    partial class TwitControl
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
            this.components = new System.ComponentModel.Container();
            this._twit = new System.Windows.Forms.TextBox();
            this._msg = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // _twit
            // 
            this._twit.Location = new System.Drawing.Point(4, 33);
            this._twit.MaxLength = 140;
            this._twit.Name = "_twit";
            this._twit.Size = new System.Drawing.Size(304, 26);
            this._twit.TabIndex = 0;
            this.toolTip1.SetToolTip(this._twit, "enter message, enter to send.");
            this._twit.KeyUp += new System.Windows.Forms.KeyEventHandler(this._twit_KeyUp);
            // 
            // _msg
            // 
            this._msg.Location = new System.Drawing.Point(4, 4);
            this._msg.Multiline = true;
            this._msg.Name = "_msg";
            this._msg.ReadOnly = true;
            this._msg.Size = new System.Drawing.Size(304, 26);
            this._msg.TabIndex = 2;
            this.toolTip1.SetToolTip(this._msg, "log");
            // 
            // TwitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._msg);
            this.Controls.Add(this._twit);
            this.Name = "TwitControl";
            this.Size = new System.Drawing.Size(316, 64);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _twit;
        private System.Windows.Forms.TextBox _msg;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
