namespace TradeLink.Research
{
    partial class FetchBasket
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
            this.components = new System.ComponentModel.Container();
            this.urlbox = new System.Windows.Forms.TextBox();
            this.urlnamebox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.okbut = new System.Windows.Forms.Button();
            this.cancelbut = new System.Windows.Forms.Button();
            this.nysebut = new System.Windows.Forms.CheckBox();
            this.nasdaqbut = new System.Windows.Forms.CheckBox();
            this.allsymbolsbut = new System.Windows.Forms.RadioButton();
            this.linkedonlybut = new System.Windows.Forms.RadioButton();
            this.fileurlbut = new System.Windows.Forms.CheckBox();
            this.tipurl = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // urlbox
            // 
            this.urlbox.AllowDrop = true;
            this.urlbox.Location = new System.Drawing.Point(66, 42);
            this.urlbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.urlbox.Name = "urlbox";
            this.urlbox.Size = new System.Drawing.Size(174, 26);
            this.urlbox.TabIndex = 0;
            this.urlbox.DoubleClick += new System.EventHandler(this.urlbox_DoubleClick);
            this.urlbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.urlbox_DragDrop);
            this.urlbox.DragOver += new System.Windows.Forms.DragEventHandler(this.urlbox_DragOver);
            // 
            // urlnamebox
            // 
            this.urlnamebox.FormattingEnabled = true;
            this.urlnamebox.Location = new System.Drawing.Point(66, 5);
            this.urlnamebox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.urlnamebox.Name = "urlnamebox";
            this.urlnamebox.Size = new System.Drawing.Size(174, 28);
            this.urlnamebox.TabIndex = 2;
            this.tipurl.SetToolTip(this.urlnamebox, "Save or restore locations");
            this.urlnamebox.SelectedIndexChanged += new System.EventHandler(this.urlnamebox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Name";
            // 
            // okbut
            // 
            this.okbut.Location = new System.Drawing.Point(71, 149);
            this.okbut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.okbut.Name = "okbut";
            this.okbut.Size = new System.Drawing.Size(42, 29);
            this.okbut.TabIndex = 4;
            this.okbut.Text = "ok";
            this.okbut.UseVisualStyleBackColor = true;
            this.okbut.Click += new System.EventHandler(this.okbut_Click);
            // 
            // cancelbut
            // 
            this.cancelbut.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbut.Location = new System.Drawing.Point(119, 149);
            this.cancelbut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cancelbut.Name = "cancelbut";
            this.cancelbut.Size = new System.Drawing.Size(64, 29);
            this.cancelbut.TabIndex = 5;
            this.cancelbut.Text = "cancel";
            this.cancelbut.UseVisualStyleBackColor = true;
            this.cancelbut.Click += new System.EventHandler(this.cancelbut_Click);
            // 
            // nysebut
            // 
            this.nysebut.AutoSize = true;
            this.nysebut.Checked = true;
            this.nysebut.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nysebut.Location = new System.Drawing.Point(12, 79);
            this.nysebut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nysebut.Name = "nysebut";
            this.nysebut.Size = new System.Drawing.Size(75, 24);
            this.nysebut.TabIndex = 6;
            this.nysebut.Text = "NYSE";
            this.nysebut.UseVisualStyleBackColor = true;
            // 
            // nasdaqbut
            // 
            this.nasdaqbut.AutoSize = true;
            this.nasdaqbut.Location = new System.Drawing.Point(12, 112);
            this.nasdaqbut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nasdaqbut.Name = "nasdaqbut";
            this.nasdaqbut.Size = new System.Drawing.Size(76, 24);
            this.nasdaqbut.TabIndex = 7;
            this.nasdaqbut.Text = "NASD";
            this.nasdaqbut.UseVisualStyleBackColor = true;
            // 
            // allsymbolsbut
            // 
            this.allsymbolsbut.AutoSize = true;
            this.allsymbolsbut.Location = new System.Drawing.Point(111, 78);
            this.allsymbolsbut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.allsymbolsbut.Name = "allsymbolsbut";
            this.allsymbolsbut.Size = new System.Drawing.Size(108, 24);
            this.allsymbolsbut.TabIndex = 8;
            this.allsymbolsbut.TabStop = true;
            this.allsymbolsbut.Text = "All symbols";
            this.tipurl.SetToolTip(this.allsymbolsbut, "Imports all found symbols");
            this.allsymbolsbut.UseVisualStyleBackColor = true;
            // 
            // linkedonlybut
            // 
            this.linkedonlybut.AutoSize = true;
            this.linkedonlybut.Checked = true;
            this.linkedonlybut.Location = new System.Drawing.Point(111, 111);
            this.linkedonlybut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.linkedonlybut.Name = "linkedonlybut";
            this.linkedonlybut.Size = new System.Drawing.Size(128, 24);
            this.linkedonlybut.TabIndex = 9;
            this.linkedonlybut.TabStop = true;
            this.linkedonlybut.Text = "Clickable Only";
            this.tipurl.SetToolTip(this.linkedonlybut, "For URLs, only hyperlinked symbols are imported");
            this.linkedonlybut.UseVisualStyleBackColor = true;
            // 
            // fileurlbut
            // 
            this.fileurlbut.Appearance = System.Windows.Forms.Appearance.Button;
            this.fileurlbut.AutoSize = true;
            this.fileurlbut.Location = new System.Drawing.Point(8, 40);
            this.fileurlbut.Name = "fileurlbut";
            this.fileurlbut.Size = new System.Drawing.Size(56, 30);
            this.fileurlbut.TabIndex = 10;
            this.fileurlbut.Text = "URL:";
            this.tipurl.SetToolTip(this.fileurlbut, "Click to get basket from file instead of by URL.");
            this.fileurlbut.UseVisualStyleBackColor = true;
            this.fileurlbut.CheckedChanged += new System.EventHandler(this.fileurlbut_CheckedChanged);
            // 
            // FetchBasket
            // 
            this.AcceptButton = this.okbut;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelbut;
            this.ClientSize = new System.Drawing.Size(248, 186);
            this.Controls.Add(this.fileurlbut);
            this.Controls.Add(this.linkedonlybut);
            this.Controls.Add(this.allsymbolsbut);
            this.Controls.Add(this.nasdaqbut);
            this.Controls.Add(this.nysebut);
            this.Controls.Add(this.cancelbut);
            this.Controls.Add(this.okbut);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.urlnamebox);
            this.Controls.Add(this.urlbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FetchBasket";
            this.Text = "Web Fetcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox urlbox;
        private System.Windows.Forms.ComboBox urlnamebox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okbut;
        private System.Windows.Forms.Button cancelbut;
        private System.Windows.Forms.CheckBox nysebut;
        private System.Windows.Forms.CheckBox nasdaqbut;
        private System.Windows.Forms.RadioButton allsymbolsbut;
        private System.Windows.Forms.RadioButton linkedonlybut;
        private System.Windows.Forms.CheckBox fileurlbut;
        private System.Windows.Forms.ToolTip tipurl;
    }
}