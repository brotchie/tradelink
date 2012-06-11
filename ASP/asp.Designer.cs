namespace ASP
{
    partial class ASP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ASP));
            this._availresponses = new System.Windows.Forms.ComboBox();
            this._resnames = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this._newrespbox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this._skins = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._saveskins = new System.Windows.Forms.Button();
            this._remskin = new System.Windows.Forms.Button();
            this._librarysel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._twithelp = new System.Windows.Forms.Button();
            this._opttog = new System.Windows.Forms.Button();
            this._togglemsgs = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this._newrespbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _availresponses
            // 
            this._availresponses.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._availresponses.FormattingEnabled = true;
            this._availresponses.Location = new System.Drawing.Point(72, 19);
            this._availresponses.Name = "_availresponses";
            this._availresponses.Size = new System.Drawing.Size(223, 21);
            this._availresponses.TabIndex = 1;
            this.toolTip1.SetToolTip(this._availresponses, "List of responses available in current response library");
            this._availresponses.SelectedIndexChanged += new System.EventHandler(this.Boxes_SelectedIndexChanged);
            // 
            // _resnames
            // 
            this._resnames.FormattingEnabled = true;
            this._resnames.HorizontalScrollbar = true;
            this._resnames.Location = new System.Drawing.Point(9, 87);
            this._resnames.Name = "_resnames";
            this._resnames.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._resnames.Size = new System.Drawing.Size(305, 95);
            this._resnames.TabIndex = 4;
            this._resnames.TabStop = false;
            this.toolTip1.SetToolTip(this._resnames, "Active Responses");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 218);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(325, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // _newrespbox
            // 
            this._newrespbox.Controls.Add(this.label2);
            this._newrespbox.Controls.Add(this._skins);
            this._newrespbox.Controls.Add(this.label1);
            this._newrespbox.Controls.Add(this._saveskins);
            this._newrespbox.Controls.Add(this._remskin);
            this._newrespbox.Controls.Add(this._availresponses);
            this._newrespbox.Location = new System.Drawing.Point(9, 8);
            this._newrespbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._newrespbox.Name = "_newrespbox";
            this._newrespbox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._newrespbox.Size = new System.Drawing.Size(303, 74);
            this._newrespbox.TabIndex = 16;
            this._newrespbox.TabStop = false;
            this._newrespbox.Text = "Start new Response";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 21);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Response: ";
            // 
            // _skins
            // 
            this._skins.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._skins.FormattingEnabled = true;
            this._skins.Location = new System.Drawing.Point(72, 42);
            this._skins.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._skins.Name = "_skins";
            this._skins.Size = new System.Drawing.Size(175, 21);
            this._skins.TabIndex = 20;
            this.toolTip1.SetToolTip(this._skins, "Skins you have saved");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 44);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Skins: ";
            // 
            // _saveskins
            // 
            this._saveskins.Location = new System.Drawing.Point(273, 42);
            this._saveskins.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._saveskins.Name = "_saveskins";
            this._saveskins.Size = new System.Drawing.Size(21, 19);
            this._saveskins.TabIndex = 22;
            this._saveskins.Text = "S";
            this.toolTip1.SetToolTip(this._saveskins, "save current settings from running skins");
            this._saveskins.UseVisualStyleBackColor = true;
            // 
            // _remskin
            // 
            this._remskin.Location = new System.Drawing.Point(250, 42);
            this._remskin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._remskin.Name = "_remskin";
            this._remskin.Size = new System.Drawing.Size(19, 19);
            this._remskin.TabIndex = 21;
            this._remskin.Text = "-";
            this.toolTip1.SetToolTip(this._remskin, "delete current skin");
            this._remskin.UseVisualStyleBackColor = true;
            // 
            // _librarysel
            // 
            this._librarysel.Location = new System.Drawing.Point(87, 186);
            this._librarysel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._librarysel.Name = "_librarysel";
            this._librarysel.Size = new System.Drawing.Size(53, 22);
            this._librarysel.TabIndex = 21;
            this._librarysel.Text = "Library";
            this.toolTip1.SetToolTip(this._librarysel, "Change library where responses are obtained");
            this._librarysel.UseVisualStyleBackColor = true;
            this._librarysel.Click += new System.EventHandler(this.LoadDLL_Click);
            // 
            // _twithelp
            // 
            this._twithelp.Image = ((System.Drawing.Image)(resources.GetObject("_twithelp.Image")));
            this._twithelp.Location = new System.Drawing.Point(291, 186);
            this._twithelp.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._twithelp.Name = "_twithelp";
            this._twithelp.Size = new System.Drawing.Size(21, 22);
            this._twithelp.TabIndex = 20;
            this.toolTip1.SetToolTip(this._twithelp, "submit bug report");
            this._twithelp.UseVisualStyleBackColor = true;
            this._twithelp.Click += new System.EventHandler(this._twithelp_Click_1);
            // 
            // _opttog
            // 
            this._opttog.Location = new System.Drawing.Point(34, 186);
            this._opttog.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._opttog.Name = "_opttog";
            this._opttog.Size = new System.Drawing.Size(23, 22);
            this._opttog.TabIndex = 21;
            this._opttog.Text = "?";
            this.toolTip1.SetToolTip(this._opttog, "change ASP options");
            this._opttog.UseVisualStyleBackColor = true;
            this._opttog.Click += new System.EventHandler(this._opttog_Click);
            // 
            // _togglemsgs
            // 
            this._togglemsgs.Location = new System.Drawing.Point(9, 186);
            this._togglemsgs.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._togglemsgs.Name = "_togglemsgs";
            this._togglemsgs.Size = new System.Drawing.Size(21, 22);
            this._togglemsgs.TabIndex = 19;
            this._togglemsgs.Text = "!";
            this._togglemsgs.UseVisualStyleBackColor = true;
            this._togglemsgs.Click += new System.EventHandler(this._togglemsgs_Click);
            // 
            // ASP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 240);
            this.Controls.Add(this._opttog);
            this.Controls.Add(this._twithelp);
            this.Controls.Add(this._librarysel);
            this.Controls.Add(this._togglemsgs);
            this.Controls.Add(this._newrespbox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this._resnames);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ASP";
            this.Text = "ASP";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this._newrespbox.ResumeLayout(false);
            this._newrespbox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _availresponses;
        private System.Windows.Forms.ListBox _resnames;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.GroupBox _newrespbox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button _togglemsgs;
        private System.Windows.Forms.Button _twithelp;
        private System.Windows.Forms.Button _librarysel;
        private System.Windows.Forms.Button _opttog;
        public System.Windows.Forms.Button _saveskins;
        public System.Windows.Forms.Button _remskin;
        public System.Windows.Forms.ComboBox _skins;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

