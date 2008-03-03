namespace Chartographer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.chartsymbolbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.blackbackground = new System.Windows.Forms.CheckBox();
            this.stickychartsbox = new System.Windows.Forms.CheckBox();
            this.maxchartbox = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chartsymbolbox
            // 
            this.chartsymbolbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.chartsymbolbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chartsymbolbox.Location = new System.Drawing.Point(38, 5);
            this.chartsymbolbox.Name = "chartsymbolbox";
            this.chartsymbolbox.Size = new System.Drawing.Size(58, 22);
            this.chartsymbolbox.TabIndex = 0;
            this.chartsymbolbox.Text = "IBM";
            this.chartsymbolbox.Click += new System.EventHandler(this.chartsymbolbox_Click);
            this.chartsymbolbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chartsymbolbox_KeyPress);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(102, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(34, 22);
            this.button1.TabIndex = 3;
            this.button1.Text = "net";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // blackbackground
            // 
            this.blackbackground.Appearance = System.Windows.Forms.Appearance.Button;
            this.blackbackground.AutoSize = true;
            this.blackbackground.Checked = global::Chartographer.Properties.Settings.Default.blackchartbg;
            this.blackbackground.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Chartographer.Properties.Settings.Default, "blackchartbg", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.blackbackground.Location = new System.Drawing.Point(3, 34);
            this.blackbackground.Name = "blackbackground";
            this.blackbackground.Size = new System.Drawing.Size(43, 23);
            this.blackbackground.TabIndex = 5;
            this.blackbackground.Text = "black";
            this.blackbackground.UseVisualStyleBackColor = true;
            this.blackbackground.CheckedChanged += new System.EventHandler(this.blackbackground_CheckedChanged);
            // 
            // stickychartsbox
            // 
            this.stickychartsbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.stickychartsbox.AutoSize = true;
            this.stickychartsbox.Checked = global::Chartographer.Properties.Settings.Default.stickychartson;
            this.stickychartsbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Chartographer.Properties.Settings.Default, "stickychartson", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.stickychartsbox.Location = new System.Drawing.Point(52, 34);
            this.stickychartsbox.Name = "stickychartsbox";
            this.stickychartsbox.Size = new System.Drawing.Size(44, 23);
            this.stickychartsbox.TabIndex = 4;
            this.stickychartsbox.Text = "sticky";
            this.stickychartsbox.UseVisualStyleBackColor = true;
            this.stickychartsbox.CheckedChanged += new System.EventHandler(this.stickychartsbox_CheckedChanged);
            // 
            // maxchartbox
            // 
            this.maxchartbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.maxchartbox.AutoSize = true;
            this.maxchartbox.Checked = global::Chartographer.Properties.Settings.Default.maxcharts;
            this.maxchartbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Chartographer.Properties.Settings.Default, "maxcharts", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.maxchartbox.Location = new System.Drawing.Point(100, 34);
            this.maxchartbox.Name = "maxchartbox";
            this.maxchartbox.Size = new System.Drawing.Size(36, 23);
            this.maxchartbox.TabIndex = 2;
            this.maxchartbox.Text = "max";
            this.maxchartbox.UseVisualStyleBackColor = true;
            this.maxchartbox.CheckedChanged += new System.EventHandler(this.maxchartbox_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(3, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(28, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "file";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(139, 60);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.blackbackground);
            this.Controls.Add(this.stickychartsbox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.maxchartbox);
            this.Controls.Add(this.chartsymbolbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Chart";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chartsymbolbox;
        private System.Windows.Forms.CheckBox maxchartbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox stickychartsbox;
        private System.Windows.Forms.CheckBox blackbackground;
        private System.Windows.Forms.Button button2;
    }
}

