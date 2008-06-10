namespace TLReplay
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
            this.inputselectbut = new System.Windows.Forms.Button();
            this.speedbar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nowplayinggrp = new System.Windows.Forms.GroupBox();
            this.currdate = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.stopbut = new System.Windows.Forms.Button();
            this.gobut = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.speedbar)).BeginInit();
            this.nowplayinggrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(134, 44);
            this.monthCalendar1.Margin = new System.Windows.Forms.Padding(12, 11, 12, 11);
            this.monthCalendar1.MaxSelectionCount = 1;
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowToday = false;
            this.monthCalendar1.ShowTodayCircle = false;
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
            // 
            // inputselectbut
            // 
            this.inputselectbut.Location = new System.Drawing.Point(338, 12);
            this.inputselectbut.Margin = new System.Windows.Forms.Padding(4);
            this.inputselectbut.Name = "inputselectbut";
            this.inputselectbut.Size = new System.Drawing.Size(65, 34);
            this.inputselectbut.TabIndex = 1;
            this.inputselectbut.Text = "Input";
            this.inputselectbut.UseVisualStyleBackColor = true;
            this.inputselectbut.Click += new System.EventHandler(this.inputselectbut_Click);
            // 
            // speedbar
            // 
            this.speedbar.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::TLReplay.Properties.Settings.Default, "speed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.speedbar.Location = new System.Drawing.Point(9, 105);
            this.speedbar.Margin = new System.Windows.Forms.Padding(4);
            this.speedbar.Name = "speedbar";
            this.speedbar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.speedbar.Size = new System.Drawing.Size(53, 129);
            this.speedbar.TabIndex = 9;
            this.speedbar.Value = global::TLReplay.Properties.Settings.Default.speed;
            this.speedbar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.speedbar_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "Speed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(64, 204);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Fast";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 110);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Slow";
            // 
            // nowplayinggrp
            // 
            this.nowplayinggrp.Controls.Add(this.currdate);
            this.nowplayinggrp.Enabled = false;
            this.nowplayinggrp.Location = new System.Drawing.Point(9, 12);
            this.nowplayinggrp.Margin = new System.Windows.Forms.Padding(4);
            this.nowplayinggrp.Name = "nowplayinggrp";
            this.nowplayinggrp.Padding = new System.Windows.Forms.Padding(4);
            this.nowplayinggrp.Size = new System.Drawing.Size(109, 47);
            this.nowplayinggrp.TabIndex = 15;
            this.nowplayinggrp.TabStop = false;
            this.nowplayinggrp.Text = "Now Playing";
            // 
            // currdate
            // 
            this.currdate.AutoSize = true;
            this.currdate.Location = new System.Drawing.Point(25, 20);
            this.currdate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currdate.Name = "currdate";
            this.currdate.Size = new System.Drawing.Size(40, 17);
            this.currdate.TabIndex = 0;
            this.currdate.Text = "none";
            this.currdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 156);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Realistic";
            // 
            // stopbut
            // 
            this.stopbut.Image = ((System.Drawing.Image)(resources.GetObject("stopbut.Image")));
            this.stopbut.Location = new System.Drawing.Point(190, 12);
            this.stopbut.Margin = new System.Windows.Forms.Padding(4);
            this.stopbut.Name = "stopbut";
            this.stopbut.Size = new System.Drawing.Size(56, 34);
            this.stopbut.TabIndex = 17;
            this.stopbut.UseVisualStyleBackColor = true;
            this.stopbut.Click += new System.EventHandler(this.stopbut_Click);
            // 
            // gobut
            // 
            this.gobut.Image = ((System.Drawing.Image)(resources.GetObject("gobut.Image")));
            this.gobut.Location = new System.Drawing.Point(134, 12);
            this.gobut.Margin = new System.Windows.Forms.Padding(4);
            this.gobut.Name = "gobut";
            this.gobut.Size = new System.Drawing.Size(48, 34);
            this.gobut.TabIndex = 7;
            this.gobut.Text = ">";
            this.gobut.UseVisualStyleBackColor = true;
            this.gobut.Click += new System.EventHandler(this.gobut_Click);
            // 
            // Replay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(416, 254);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.stopbut);
            this.Controls.Add(this.nowplayinggrp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gobut);
            this.Controls.Add(this.inputselectbut);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.speedbar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Replay";
            this.Text = "TradeLink Replay";
            ((System.ComponentModel.ISupportInitialize)(this.speedbar)).EndInit();
            this.nowplayinggrp.ResumeLayout(false);
            this.nowplayinggrp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Button inputselectbut;
        private System.Windows.Forms.Button gobut;
        private System.Windows.Forms.TrackBar speedbar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox nowplayinggrp;
        private System.Windows.Forms.Button stopbut;
        private System.Windows.Forms.Label currdate;
        private System.Windows.Forms.Label label4;
    }
}

