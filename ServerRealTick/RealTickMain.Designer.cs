namespace RealTickConnector
{
    partial class RealTickMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RealTickMain));
            this._report = new System.Windows.Forms.Button();
            this._msg = new System.Windows.Forms.Button();
            this._start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _report
            // 
            this._report.Image = ((System.Drawing.Image)(resources.GetObject("_report.Image")));
            this._report.Location = new System.Drawing.Point(230, 54);
            this._report.Name = "_report";
            this._report.Size = new System.Drawing.Size(26, 31);
            this._report.TabIndex = 18;
            this._report.UseVisualStyleBackColor = true;
            this._report.Click += new System.EventHandler(this._report_Click);
            // 
            // _msg
            // 
            this._msg.Location = new System.Drawing.Point(194, 54);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(30, 31);
            this._msg.TabIndex = 17;
            this._msg.Text = "!";
            this._msg.UseVisualStyleBackColor = true;
            this._msg.Click += new System.EventHandler(this._msg_Click);
            // 
            // _start
            // 
            this._start.Location = new System.Drawing.Point(111, 54);
            this._start.Name = "_start";
            this._start.Size = new System.Drawing.Size(75, 31);
            this._start.TabIndex = 16;
            this._start.Text = "Start";
            this._start.UseVisualStyleBackColor = true;
            this._start.Click += new System.EventHandler(this._start_Click);
            // 
            // RealTickMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 111);
            this.Controls.Add(this._report);
            this.Controls.Add(this._msg);
            this.Controls.Add(this._start);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "RealTickMain";
            this.Text = "RealTickServer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _report;
        private System.Windows.Forms.Button _msg;
        private System.Windows.Forms.Button _start;
    }
}

