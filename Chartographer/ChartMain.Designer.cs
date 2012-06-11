namespace Chartographer
{
    partial class ChartMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartMain));
            this.chartsymbolbox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.usecachebut = new System.Windows.Forms.CheckBox();
            this.blackbackground = new System.Windows.Forms.CheckBox();
            this.stickychartsbox = new System.Windows.Forms.CheckBox();
            this.maxchartbox = new System.Windows.Forms.CheckBox();
            this.msgbut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chartsymbolbox
            // 
            this.chartsymbolbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.chartsymbolbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this.chartsymbolbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chartsymbolbox.Location = new System.Drawing.Point(9, 6);
            this.chartsymbolbox.Name = "chartsymbolbox";
            this.chartsymbolbox.Size = new System.Drawing.Size(72, 22);
            this.chartsymbolbox.TabIndex = 0;
            this.chartsymbolbox.Text = "GOOG";
            this.toolTip1.SetToolTip(this.chartsymbolbox, "load yearly chart for desired symbol from internet (free)");
            this.chartsymbolbox.Click += new System.EventHandler(this.chartsymbolbox_Click);
            this.chartsymbolbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.chartsymbolbox_KeyUp);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(139, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(51, 22);
            this.button2.TabIndex = 6;
            this.button2.Text = "intra";
            this.toolTip1.SetToolTip(this.button2, "load intraday chart from tick file");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(86, 6);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 22);
            this.button1.TabIndex = 7;
            this.button1.Text = "daily";
            this.toolTip1.SetToolTip(this.button1, "load daily chart for desired symbol using internet data (free)");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // usecachebut
            // 
            this.usecachebut.Appearance = System.Windows.Forms.Appearance.Button;
            this.usecachebut.AutoSize = true;
            this.usecachebut.Checked = global::Chartographer.Properties.Settings.Default.usecache;
            this.usecachebut.CheckState = System.Windows.Forms.CheckState.Checked;
            this.usecachebut.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Chartographer.Properties.Settings.Default, "usecache", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.usecachebut.Location = new System.Drawing.Point(165, 34);
            this.usecachebut.Name = "usecachebut";
            this.usecachebut.Size = new System.Drawing.Size(64, 23);
            this.usecachebut.TabIndex = 8;
            this.usecachebut.Text = "usecache";
            this.toolTip1.SetToolTip(this.usecachebut, "maximize new charts");
            this.usecachebut.UseVisualStyleBackColor = true;
            // 
            // blackbackground
            // 
            this.blackbackground.Appearance = System.Windows.Forms.Appearance.Button;
            this.blackbackground.AutoSize = true;
            this.blackbackground.Checked = global::Chartographer.Properties.Settings.Default.blackchartbg;
            this.blackbackground.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Chartographer.Properties.Settings.Default, "blackchartbg", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.blackbackground.Location = new System.Drawing.Point(9, 34);
            this.blackbackground.Name = "blackbackground";
            this.blackbackground.Size = new System.Drawing.Size(43, 23);
            this.blackbackground.TabIndex = 5;
            this.blackbackground.Text = "black";
            this.toolTip1.SetToolTip(this.blackbackground, "blackground is white unless black is checked");
            this.blackbackground.UseVisualStyleBackColor = true;
            this.blackbackground.CheckedChanged += new System.EventHandler(this.blackbackground_CheckedChanged);
            // 
            // stickychartsbox
            // 
            this.stickychartsbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.stickychartsbox.AutoSize = true;
            this.stickychartsbox.Checked = global::Chartographer.Properties.Settings.Default.stickychartson;
            this.stickychartsbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Chartographer.Properties.Settings.Default, "stickychartson", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.stickychartsbox.Location = new System.Drawing.Point(58, 34);
            this.stickychartsbox.Name = "stickychartsbox";
            this.stickychartsbox.Size = new System.Drawing.Size(44, 23);
            this.stickychartsbox.TabIndex = 4;
            this.stickychartsbox.Text = "sticky";
            this.toolTip1.SetToolTip(this.stickychartsbox, "remember position of this window on restart");
            this.stickychartsbox.UseVisualStyleBackColor = true;
            this.stickychartsbox.CheckedChanged += new System.EventHandler(this.stickychartsbox_CheckedChanged);
            // 
            // maxchartbox
            // 
            this.maxchartbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.maxchartbox.AutoSize = true;
            this.maxchartbox.Checked = global::Chartographer.Properties.Settings.Default.maxcharts;
            this.maxchartbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Chartographer.Properties.Settings.Default, "maxcharts", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.maxchartbox.Location = new System.Drawing.Point(111, 34);
            this.maxchartbox.Name = "maxchartbox";
            this.maxchartbox.Size = new System.Drawing.Size(48, 23);
            this.maxchartbox.TabIndex = 2;
            this.maxchartbox.Text = "maxed";
            this.toolTip1.SetToolTip(this.maxchartbox, "maximize new charts");
            this.maxchartbox.UseVisualStyleBackColor = true;
            this.maxchartbox.CheckedChanged += new System.EventHandler(this.maxchartbox_CheckedChanged);
            // 
            // msgbut
            // 
            this.msgbut.Location = new System.Drawing.Point(208, 5);
            this.msgbut.Name = "msgbut";
            this.msgbut.Size = new System.Drawing.Size(21, 23);
            this.msgbut.TabIndex = 9;
            this.msgbut.Text = "!";
            this.msgbut.UseVisualStyleBackColor = true;
            this.msgbut.Click += new System.EventHandler(this.msgbut_Click);
            // 
            // ChartMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(244, 72);
            this.Controls.Add(this.msgbut);
            this.Controls.Add(this.usecachebut);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.blackbackground);
            this.Controls.Add(this.stickychartsbox);
            this.Controls.Add(this.maxchartbox);
            this.Controls.Add(this.chartsymbolbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChartMain";
            this.Text = "Chartographer";
            this.TopMost = true;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ChartMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ChartMain_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chartsymbolbox;
        private System.Windows.Forms.CheckBox maxchartbox;
        private System.Windows.Forms.CheckBox stickychartsbox;
        private System.Windows.Forms.CheckBox blackbackground;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox usecachebut;
        private System.Windows.Forms.Button msgbut;
    }
}

