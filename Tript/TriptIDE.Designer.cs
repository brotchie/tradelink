namespace Tript
{
    partial class TriptIDE
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriptIDE));
            this.editorbox = new System.Windows.Forms.RichTextBox();
            this.outbox = new System.Windows.Forms.RichTextBox();
            this.runbut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // editorbox
            // 
            this.editorbox.AcceptsTab = true;
            this.editorbox.Location = new System.Drawing.Point(13, 13);
            this.editorbox.Name = "editorbox";
            this.editorbox.Size = new System.Drawing.Size(457, 207);
            this.editorbox.TabIndex = 0;
            this.editorbox.Text = "";
            // 
            // outbox
            // 
            this.outbox.Location = new System.Drawing.Point(13, 280);
            this.outbox.Name = "outbox";
            this.outbox.ReadOnly = true;
            this.outbox.Size = new System.Drawing.Size(570, 85);
            this.outbox.TabIndex = 1;
            this.outbox.Text = "";
            // 
            // runbut
            // 
            this.runbut.Location = new System.Drawing.Point(477, 13);
            this.runbut.Name = "runbut";
            this.runbut.Size = new System.Drawing.Size(75, 23);
            this.runbut.TabIndex = 2;
            this.runbut.Text = "Run";
            this.runbut.UseVisualStyleBackColor = true;
            this.runbut.Click += new System.EventHandler(this.runbut_Click);
            // 
            // TriptIDE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 377);
            this.Controls.Add(this.runbut);
            this.Controls.Add(this.outbox);
            this.Controls.Add(this.editorbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TriptIDE";
            this.Text = "Tript";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox editorbox;
        private System.Windows.Forms.RichTextBox outbox;
        private System.Windows.Forms.Button runbut;
    }
}

