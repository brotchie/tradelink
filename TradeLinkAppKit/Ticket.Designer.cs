using TradeLink.Common;

namespace TradeLink.AppKit
{
    partial class Ticket
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
            this.sendbut = new System.Windows.Forms.Button();
            this.obuybut = new System.Windows.Forms.RadioButton();
            this.osellbut = new System.Windows.Forms.RadioButton();
            this.pricelabel = new System.Windows.Forms.Label();
            this.limitbut = new System.Windows.Forms.RadioButton();
            this.stopbut = new System.Windows.Forms.RadioButton();
            this.marketbut = new System.Windows.Forms.RadioButton();
            this.oprice = new System.Windows.Forms.NumericUpDown();
            this.osize = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.oprice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.osize)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sendbut
            // 
            this.sendbut.BackColor = System.Drawing.SystemColors.Window;
            this.sendbut.FlatAppearance.BorderSize = 0;
            this.sendbut.Location = new System.Drawing.Point(163, 4);
            this.sendbut.Margin = new System.Windows.Forms.Padding(0);
            this.sendbut.Name = "sendbut";
            this.sendbut.Size = new System.Drawing.Size(66, 36);
            this.sendbut.TabIndex = 0;
            this.sendbut.Text = "Send";
            this.sendbut.UseVisualStyleBackColor = false;
            this.sendbut.Click += new System.EventHandler(this.limitbut_Click);
            // 
            // obuybut
            // 
            this.obuybut.AutoSize = true;
            this.obuybut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.obuybut.Location = new System.Drawing.Point(7, 0);
            this.obuybut.Margin = new System.Windows.Forms.Padding(0);
            this.obuybut.Name = "obuybut";
            this.obuybut.Size = new System.Drawing.Size(59, 24);
            this.obuybut.TabIndex = 4;
            this.obuybut.Text = "Buy";
            this.obuybut.UseVisualStyleBackColor = true;
            this.obuybut.CheckedChanged += new System.EventHandler(this.obuybut_CheckedChanged);
            // 
            // osellbut
            // 
            this.osellbut.AutoSize = true;
            this.osellbut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.osellbut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.osellbut.Location = new System.Drawing.Point(85, 0);
            this.osellbut.Margin = new System.Windows.Forms.Padding(0);
            this.osellbut.Name = "osellbut";
            this.osellbut.Size = new System.Drawing.Size(58, 24);
            this.osellbut.TabIndex = 5;
            this.osellbut.Text = "Sell";
            this.osellbut.UseVisualStyleBackColor = true;
            this.osellbut.CheckedChanged += new System.EventHandler(this.osellbut_CheckedChanged);
            // 
            // pricelabel
            // 
            this.pricelabel.AutoSize = true;
            this.pricelabel.Location = new System.Drawing.Point(22, 65);
            this.pricelabel.Margin = new System.Windows.Forms.Padding(0);
            this.pricelabel.Name = "pricelabel";
            this.pricelabel.Size = new System.Drawing.Size(48, 20);
            this.pricelabel.TabIndex = 6;
            this.pricelabel.Text = "Price";
            this.pricelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // limitbut
            // 
            this.limitbut.AutoSize = true;
            this.limitbut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.limitbut.Checked = true;
            this.limitbut.Location = new System.Drawing.Point(11, 39);
            this.limitbut.Margin = new System.Windows.Forms.Padding(0);
            this.limitbut.Name = "limitbut";
            this.limitbut.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.limitbut.Size = new System.Drawing.Size(59, 24);
            this.limitbut.TabIndex = 10;
            this.limitbut.TabStop = true;
            this.limitbut.Text = "Lmt";
            this.limitbut.UseVisualStyleBackColor = true;
            // 
            // stopbut
            // 
            this.stopbut.AutoSize = true;
            this.stopbut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.stopbut.Location = new System.Drawing.Point(92, 39);
            this.stopbut.Margin = new System.Windows.Forms.Padding(0);
            this.stopbut.Name = "stopbut";
            this.stopbut.Size = new System.Drawing.Size(55, 24);
            this.stopbut.TabIndex = 11;
            this.stopbut.Text = "Stp";
            this.stopbut.UseVisualStyleBackColor = true;
            // 
            // marketbut
            // 
            this.marketbut.AutoSize = true;
            this.marketbut.Location = new System.Drawing.Point(163, 39);
            this.marketbut.Margin = new System.Windows.Forms.Padding(0);
            this.marketbut.Name = "marketbut";
            this.marketbut.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.marketbut.Size = new System.Drawing.Size(57, 24);
            this.marketbut.TabIndex = 15;
            this.marketbut.TabStop = true;
            this.marketbut.Text = "Mkt";
            this.marketbut.UseVisualStyleBackColor = true;
            // 
            // oprice
            // 
            this.oprice.DecimalPlaces = 2;
            this.oprice.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.oprice.Location = new System.Drawing.Point(74, 63);
            this.oprice.Margin = new System.Windows.Forms.Padding(0);
            this.oprice.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.oprice.Name = "oprice";
            this.oprice.Size = new System.Drawing.Size(82, 26);
            this.oprice.TabIndex = 16;
            this.oprice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.oprice.KeyUp += new System.Windows.Forms.KeyEventHandler(this.oprice_KeyUp);
            this.oprice.MouseUp += new System.Windows.Forms.MouseEventHandler(this.oprice_MouseUp);
            // 
            // osize
            // 
            this.osize.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.osize.Location = new System.Drawing.Point(156, 63);
            this.osize.Margin = new System.Windows.Forms.Padding(0);
            this.osize.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.osize.Name = "osize";
            this.osize.Size = new System.Drawing.Size(73, 26);
            this.osize.TabIndex = 17;
            this.osize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.osize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.osize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.osize_MouseWheel);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.obuybut);
            this.panel1.Controls.Add(this.osellbut);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(4, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(152, 24);
            this.panel1.TabIndex = 18;
            // 
            // Ticket
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(238, 104);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.osize);
            this.Controls.Add(this.oprice);
            this.Controls.Add(this.marketbut);
            this.Controls.Add(this.stopbut);
            this.Controls.Add(this.limitbut);
            this.Controls.Add(this.pricelabel);
            this.Controls.Add(this.sendbut);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Ticket";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "order";
            this.TopMost = true;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.order_MouseWheel);
            ((System.ComponentModel.ISupportInitialize)(this.oprice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.osize)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendbut;
        private System.Windows.Forms.RadioButton obuybut;
        private System.Windows.Forms.RadioButton osellbut;
        private System.Windows.Forms.Label pricelabel;
        private System.Windows.Forms.RadioButton limitbut;
        private System.Windows.Forms.RadioButton stopbut;
        private System.Windows.Forms.RadioButton marketbut;
        private System.Windows.Forms.NumericUpDown oprice;
        private System.Windows.Forms.NumericUpDown osize;
        private System.Windows.Forms.Panel panel1;
    }
}