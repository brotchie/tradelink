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
            this._create = new System.Windows.Forms.Button();
            this._space = new System.Windows.Forms.TextBox();
            this._user = new System.Windows.Forms.TextBox();
            this._pass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._desc = new System.Windows.Forms.TextBox();
            this._stat = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._summ = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _create
            // 
            this._create.Location = new System.Drawing.Point(8, 228);
            this._create.Name = "_create";
            this._create.Size = new System.Drawing.Size(117, 31);
            this._create.TabIndex = 0;
            this._create.Text = "create ticket";
            this._create.UseVisualStyleBackColor = true;
            this._create.Click += new System.EventHandler(this._create_Click);
            // 
            // _space
            // 
            this._space.Location = new System.Drawing.Point(69, 15);
            this._space.Name = "_space";
            this._space.Size = new System.Drawing.Size(74, 26);
            this._space.TabIndex = 1;
            this.toolTip1.SetToolTip(this._space, "space name on assembla (project name)");
            // 
            // _user
            // 
            this._user.Location = new System.Drawing.Point(202, 15);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(73, 26);
            this._user.TabIndex = 2;
            this.toolTip1.SetToolTip(this._user, "email address you login to assembla with");
            // 
            // _pass
            // 
            this._pass.Location = new System.Drawing.Point(335, 15);
            this._pass.Name = "_pass";
            this._pass.PasswordChar = '*';
            this._pass.Size = new System.Drawing.Size(73, 26);
            this._pass.TabIndex = 3;
            this.toolTip1.SetToolTip(this._pass, "password of your assembla account");
            this._pass.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Space:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(149, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "User:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(281, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Pass:";
            // 
            // _desc
            // 
            this._desc.Location = new System.Drawing.Point(5, 85);
            this._desc.Multiline = true;
            this._desc.Name = "_desc";
            this._desc.Size = new System.Drawing.Size(403, 137);
            this._desc.TabIndex = 7;
            this.toolTip1.SetToolTip(this._desc, "description of your ticket");
            this._desc.WordWrap = false;
            // 
            // _stat
            // 
            this._stat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._stat.Location = new System.Drawing.Point(131, 228);
            this._stat.Multiline = true;
            this._stat.Name = "_stat";
            this._stat.ReadOnly = true;
            this._stat.Size = new System.Drawing.Size(289, 31);
            this._stat.TabIndex = 8;
            this._stat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Subject:";
            // 
            // _summ
            // 
            this._summ.Location = new System.Drawing.Point(69, 47);
            this._summ.Name = "_summ";
            this._summ.Size = new System.Drawing.Size(339, 26);
            this._summ.TabIndex = 10;
            // 
            // AssemblaTicketControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._summ);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._stat);
            this.Controls.Add(this._desc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._pass);
            this.Controls.Add(this._user);
            this.Controls.Add(this._space);
            this.Controls.Add(this._create);
            this.Name = "AssemblaTicketControl";
            this.Size = new System.Drawing.Size(420, 271);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button _create;
        public System.Windows.Forms.TextBox _space;
        public System.Windows.Forms.TextBox _user;
        public System.Windows.Forms.TextBox _pass;
        public System.Windows.Forms.TextBox _desc;
        private System.Windows.Forms.TextBox _stat;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox _summ;
    }
}
