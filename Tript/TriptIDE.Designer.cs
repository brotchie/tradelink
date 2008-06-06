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
            this.loadbut = new System.Windows.Forms.Button();
            this.savebut = new System.Windows.Forms.Button();
            this.saveas = new System.Windows.Forms.Button();
            this.newbut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // editorbox
            // 
            this.editorbox.AcceptsTab = true;
            this.editorbox.Location = new System.Drawing.Point(13, 13);
            this.editorbox.Name = "editorbox";
            this.editorbox.Size = new System.Drawing.Size(516, 216);
            this.editorbox.TabIndex = 0;
            this.editorbox.Text = "";
            this.editorbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.editorbox_KeyUp);
            // 
            // outbox
            // 
            this.outbox.Location = new System.Drawing.Point(13, 264);
            this.outbox.Name = "outbox";
            this.outbox.ReadOnly = true;
            this.outbox.Size = new System.Drawing.Size(570, 101);
            this.outbox.TabIndex = 1;
            this.outbox.Text = "";
            // 
            // runbut
            // 
            this.runbut.Location = new System.Drawing.Point(13, 235);
            this.runbut.Name = "runbut";
            this.runbut.Size = new System.Drawing.Size(48, 23);
            this.runbut.TabIndex = 2;
            this.runbut.Text = "Run";
            this.runbut.UseVisualStyleBackColor = true;
            this.runbut.Click += new System.EventHandler(this.runbut_Click);
            // 
            // loadbut
            // 
            this.loadbut.Location = new System.Drawing.Point(535, 99);
            this.loadbut.Name = "loadbut";
            this.loadbut.Size = new System.Drawing.Size(51, 23);
            this.loadbut.TabIndex = 3;
            this.loadbut.Text = "Load";
            this.loadbut.UseVisualStyleBackColor = true;
            this.loadbut.Click += new System.EventHandler(this.loadbut_Click);
            // 
            // savebut
            // 
            this.savebut.Location = new System.Drawing.Point(535, 41);
            this.savebut.Name = "savebut";
            this.savebut.Size = new System.Drawing.Size(48, 23);
            this.savebut.TabIndex = 4;
            this.savebut.Text = "Save";
            this.savebut.UseVisualStyleBackColor = true;
            this.savebut.Click += new System.EventHandler(this.savebut_Click);
            // 
            // saveas
            // 
            this.saveas.Location = new System.Drawing.Point(535, 70);
            this.saveas.Name = "saveas";
            this.saveas.Size = new System.Drawing.Size(51, 23);
            this.saveas.TabIndex = 5;
            this.saveas.Text = "As";
            this.saveas.UseVisualStyleBackColor = true;
            this.saveas.Click += new System.EventHandler(this.saveas_Click);
            // 
            // newbut
            // 
            this.newbut.Location = new System.Drawing.Point(535, 12);
            this.newbut.Name = "newbut";
            this.newbut.Size = new System.Drawing.Size(48, 23);
            this.newbut.TabIndex = 6;
            this.newbut.Text = "New";
            this.newbut.UseVisualStyleBackColor = true;
            this.newbut.Click += new System.EventHandler(this.newbut_Click);
            // 
            // TriptIDE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 377);
            this.Controls.Add(this.newbut);
            this.Controls.Add(this.saveas);
            this.Controls.Add(this.savebut);
            this.Controls.Add(this.loadbut);
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
        private System.Windows.Forms.Button loadbut;
        private System.Windows.Forms.Button savebut;
        private System.Windows.Forms.Button saveas;
        private System.Windows.Forms.Button newbut;
    }
}

