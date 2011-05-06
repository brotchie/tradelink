namespace TradeLink.AppKit
{
    partial class AssemblaTicketControl
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
            this.create = new System.Windows.Forms.Button();
            this.space = new System.Windows.Forms.TextBox();
            this.user = new System.Windows.Forms.TextBox();
            this.pass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.desc = new System.Windows.Forms.TextBox();
            this._ss = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.summ = new System.Windows.Forms.TextBox();
            this._stat = new System.Windows.Forms.Label();
            this._screencast = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // create
            // 
            this.create.Location = new System.Drawing.Point(347, 76);
            this.create.Name = "create";
            this.create.Size = new System.Drawing.Size(63, 35);
            this.create.TabIndex = 7;
            this.create.Text = "send";
            this.toolTip1.SetToolTip(this.create, "login to assembla and create ticket");
            this.create.UseVisualStyleBackColor = true;
            this.create.Click += new System.EventHandler(this._create_Click);
            // 
            // space
            // 
            this.space.Location = new System.Drawing.Point(71, 12);
            this.space.Name = "space";
            this.space.Size = new System.Drawing.Size(74, 26);
            this.space.TabIndex = 1;
            this.toolTip1.SetToolTip(this.space, "space name on assembla (project name)");
            // 
            // user
            // 
            this.user.Location = new System.Drawing.Point(204, 12);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(73, 26);
            this.user.TabIndex = 2;
            this.toolTip1.SetToolTip(this.user, "username you login to assembla with");
            // 
            // pass
            // 
            this.pass.Location = new System.Drawing.Point(337, 10);
            this.pass.Name = "pass";
            this.pass.PasswordChar = '*';
            this.pass.Size = new System.Drawing.Size(73, 26);
            this.pass.TabIndex = 3;
            this.toolTip1.SetToolTip(this.pass, "password of your assembla account");
            this.pass.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Space:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "User:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(283, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Pass:";
            // 
            // desc
            // 
            this.desc.Location = new System.Drawing.Point(10, 117);
            this.desc.Multiline = true;
            this.desc.Name = "desc";
            this.desc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.desc.Size = new System.Drawing.Size(403, 170);
            this.desc.TabIndex = 5;
            this.toolTip1.SetToolTip(this.desc, "description of your ticket");
            // 
            // _ss
            // 
            this._ss.Location = new System.Drawing.Point(10, 76);
            this._ss.Name = "_ss";
            this._ss.Size = new System.Drawing.Size(152, 35);
            this._ss.TabIndex = 6;
            this._ss.Text = "take screenshot";
            this.toolTip1.SetToolTip(this._ss, "take screenshot now and attach.   make sure this tool is on same monitor that you" +
                    " want the screenshot for.");
            this._ss.UseVisualStyleBackColor = true;
            this._ss.Click += new System.EventHandler(this._ss_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Summary: ";
            // 
            // summ
            // 
            this.summ.Location = new System.Drawing.Point(96, 44);
            this.summ.Name = "summ";
            this.summ.Size = new System.Drawing.Size(314, 26);
            this.summ.TabIndex = 4;
            // 
            // _stat
            // 
            this._stat.AutoSize = true;
            this._stat.ForeColor = System.Drawing.Color.ForestGreen;
            this._stat.Location = new System.Drawing.Point(309, 199);
            this._stat.Name = "_stat";
            this._stat.Size = new System.Drawing.Size(0, 20);
            this._stat.TabIndex = 11;
            // 
            // _screencast
            // 
            this._screencast.Location = new System.Drawing.Point(169, 76);
            this._screencast.Name = "_screencast";
            this._screencast.Size = new System.Drawing.Size(172, 35);
            this._screencast.TabIndex = 12;
            this._screencast.Text = "make screencast";
            this._screencast.UseVisualStyleBackColor = true;
            this._screencast.Click += new System.EventHandler(this._screencast_Click);
            // 
            // AssemblaTicketControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._screencast);
            this.Controls.Add(this._ss);
            this.Controls.Add(this._stat);
            this.Controls.Add(this.summ);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.desc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pass);
            this.Controls.Add(this.user);
            this.Controls.Add(this.space);
            this.Controls.Add(this.create);
            this.Name = "AssemblaTicketControl";
            this.Size = new System.Drawing.Size(435, 315);
            this.Load += new System.EventHandler(this.AssemblaTicketControl_Load);
            this.SizeChanged += new System.EventHandler(this.AssemblaTicketControl_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button create;
        public System.Windows.Forms.TextBox space;
        public System.Windows.Forms.TextBox user;
        public System.Windows.Forms.TextBox pass;
        public System.Windows.Forms.TextBox desc;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox summ;
        private System.Windows.Forms.Label _stat;
        private System.Windows.Forms.Button _ss;
        private System.Windows.Forms.Button _screencast;
    }
}
