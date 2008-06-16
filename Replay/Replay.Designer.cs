namespace Replay
{
    partial class Replay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Replay));
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.playbut = new System.Windows.Forms.Button();
            this.stopbut = new System.Windows.Forms.Button();
            this.inputbut = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statuslab = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressbar = new System.Windows.Forms.ToolStripProgressBar();
            this.daystartpicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(138, 42);
            this.monthCalendar1.MaxSelectionCount = 1;
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowToday = false;
            this.monthCalendar1.TabIndex = 0;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(20, 32);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(53, 142);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.Value = 5;
            // 
            // playbut
            // 
            this.playbut.Location = new System.Drawing.Point(12, 13);
            this.playbut.Name = "playbut";
            this.playbut.Size = new System.Drawing.Size(53, 23);
            this.playbut.TabIndex = 2;
            this.playbut.Text = "Play";
            this.playbut.UseVisualStyleBackColor = true;
            this.playbut.Click += new System.EventHandler(this.playbut_Click);
            // 
            // stopbut
            // 
            this.stopbut.Location = new System.Drawing.Point(72, 13);
            this.stopbut.Name = "stopbut";
            this.stopbut.Size = new System.Drawing.Size(54, 23);
            this.stopbut.TabIndex = 3;
            this.stopbut.Text = "Stop";
            this.stopbut.UseVisualStyleBackColor = true;
            this.stopbut.Click += new System.EventHandler(this.stopbut_Click);
            // 
            // inputbut
            // 
            this.inputbut.Location = new System.Drawing.Point(319, 11);
            this.inputbut.Name = "inputbut";
            this.inputbut.Size = new System.Drawing.Size(60, 23);
            this.inputbut.TabIndex = 4;
            this.inputbut.Text = "Input";
            this.inputbut.UseVisualStyleBackColor = true;
            this.inputbut.Click += new System.EventHandler(this.inputbut_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslab,
            this.progressbar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 253);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(396, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statuslab
            // 
            this.statuslab.BackColor = System.Drawing.SystemColors.Control;
            this.statuslab.Name = "statuslab";
            this.statuslab.Size = new System.Drawing.Size(279, 17);
            this.statuslab.Spring = true;
            this.statuslab.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressbar
            // 
            this.progressbar.BackColor = System.Drawing.SystemColors.Control;
            this.progressbar.Name = "progressbar";
            this.progressbar.Size = new System.Drawing.Size(100, 16);
            this.progressbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // daystartpicker
            // 
            this.daystartpicker.CustomFormat = "hh:mm tt";
            this.daystartpicker.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.daystartpicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.daystartpicker.Location = new System.Drawing.Point(229, 14);
            this.daystartpicker.Name = "daystartpicker";
            this.daystartpicker.ShowUpDown = true;
            this.daystartpicker.Size = new System.Drawing.Size(83, 22);
            this.daystartpicker.TabIndex = 6;
            this.daystartpicker.Value = new System.DateTime(2008, 6, 12, 9, 30, 0, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Real";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "DayStart";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(63, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Slow";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(65, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Fast";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Location = new System.Drawing.Point(12, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(114, 180);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Playback";
            // 
            // Replay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(396, 275);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.daystartpicker);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.inputbut);
            this.Controls.Add(this.stopbut);
            this.Controls.Add(this.playbut);
            this.Controls.Add(this.monthCalendar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Replay";
            this.Text = "TradeLink Replay";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button playbut;
        private System.Windows.Forms.Button stopbut;
        private System.Windows.Forms.Button inputbut;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statuslab;
        private System.Windows.Forms.ToolStripProgressBar progressbar;
        private System.Windows.Forms.DateTimePicker daystartpicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

