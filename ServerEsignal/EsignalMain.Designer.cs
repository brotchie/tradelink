namespace ServerEsignal
{
    partial class EsignalMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EsignalMain));
            this.label1 = new System.Windows.Forms.Label();
            this._acctapp = new System.Windows.Forms.TextBox();
            this._ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Account/App:";
            // 
            // _acctapp
            // 
            this._acctapp.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerEsignal.Properties.Settings.Default, "accountorappname", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._acctapp.Location = new System.Drawing.Point(130, 36);
            this._acctapp.Name = "_acctapp";
            this._acctapp.Size = new System.Drawing.Size(100, 26);
            this._acctapp.TabIndex = 0;
            this._acctapp.Text = global::ServerEsignal.Properties.Settings.Default.accountorappname;
            // 
            // _ok
            // 
            this._ok.Location = new System.Drawing.Point(237, 35);
            this._ok.Name = "_ok";
            this._ok.Size = new System.Drawing.Size(55, 27);
            this._ok.TabIndex = 2;
            this._ok.Text = "ok";
            this._ok.UseVisualStyleBackColor = true;
            this._ok.Click += new System.EventHandler(this._ok_Click);
            // 
            // EsignalMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 104);
            this.Controls.Add(this._ok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._acctapp);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EsignalMain";
            this.Text = "ServerEsignal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _acctapp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _ok;
    }
}

