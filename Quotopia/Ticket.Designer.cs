namespace Quotopia
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.marketbut = new System.Windows.Forms.RadioButton();
            this.oprice = new System.Windows.Forms.NumericUpDown();
            this.osize = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.oprice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.osize)).BeginInit();
            this.SuspendLayout();
            // 
            // sendbut
            // 
            this.sendbut.BackColor = System.Drawing.SystemColors.Window;
            this.sendbut.FlatAppearance.BorderSize = 0;
            this.sendbut.Location = new System.Drawing.Point(155, 4);
            this.sendbut.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.sendbut.Name = "sendbut";
            this.sendbut.Size = new System.Drawing.Size(50, 23);
            this.sendbut.TabIndex = 0;
            this.sendbut.Text = "Send";
            this.sendbut.UseVisualStyleBackColor = false;
            this.sendbut.Click += new System.EventHandler(this.limitbut_Click);
            // 
            // obuybut
            // 
            this.obuybut.AutoSize = true;
            this.obuybut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.obuybut.Location = new System.Drawing.Point(3, 12);
            this.obuybut.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.obuybut.Name = "obuybut";
            this.obuybut.Size = new System.Drawing.Size(56, 24);
            this.obuybut.TabIndex = 4;
            this.obuybut.Text = "Buy";
            this.obuybut.UseVisualStyleBackColor = true;
            this.obuybut.CheckedChanged += new System.EventHandler(this.obuybut_CheckedChanged);
            // 
            // osellbut
            // 
            this.osellbut.AutoSize = true;
            this.osellbut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.osellbut.Location = new System.Drawing.Point(70, 11);
            this.osellbut.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.osellbut.Name = "osellbut";
            this.osellbut.Size = new System.Drawing.Size(55, 24);
            this.osellbut.TabIndex = 5;
            this.osellbut.Text = "Sell";
            this.osellbut.UseVisualStyleBackColor = true;
            this.osellbut.CheckedChanged += new System.EventHandler(this.osellbut_CheckedChanged);
            // 
            // pricelabel
            // 
            this.pricelabel.AutoSize = true;
            this.pricelabel.Location = new System.Drawing.Point(4, 63);
            this.pricelabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pricelabel.Name = "pricelabel";
            this.pricelabel.Size = new System.Drawing.Size(48, 20);
            this.pricelabel.TabIndex = 6;
            this.pricelabel.Text = "Price";
            // 
            // limitbut
            // 
            this.limitbut.AutoSize = true;
            this.limitbut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.limitbut.Checked = true;
            this.limitbut.Location = new System.Drawing.Point(2, 39);
            this.limitbut.Name = "limitbut";
            this.limitbut.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.limitbut.Size = new System.Drawing.Size(64, 24);
            this.limitbut.TabIndex = 10;
            this.limitbut.TabStop = true;
            this.limitbut.Text = "Limit";
            this.limitbut.UseVisualStyleBackColor = true;
            // 
            // stopbut
            // 
            this.stopbut.AutoSize = true;
            this.stopbut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.stopbut.Location = new System.Drawing.Point(68, 38);
            this.stopbut.Name = "stopbut";
            this.stopbut.Size = new System.Drawing.Size(61, 24);
            this.stopbut.TabIndex = 11;
            this.stopbut.Text = "Stop";
            this.stopbut.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.obuybut);
            this.groupBox1.Controls.Add(this.osellbut);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.groupBox1.Location = new System.Drawing.Point(4, -5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(134, 37);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // marketbut
            // 
            this.marketbut.AutoSize = true;
            this.marketbut.Location = new System.Drawing.Point(131, 38);
            this.marketbut.Name = "marketbut";
            this.marketbut.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.marketbut.Size = new System.Drawing.Size(78, 24);
            this.marketbut.TabIndex = 15;
            this.marketbut.TabStop = true;
            this.marketbut.Text = "Market";
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
            this.oprice.Location = new System.Drawing.Point(49, 60);
            this.oprice.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.oprice.Name = "oprice";
            this.oprice.Size = new System.Drawing.Size(76, 26);
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
            this.osize.Location = new System.Drawing.Point(131, 60);
            this.osize.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.osize.Name = "osize";
            this.osize.Size = new System.Drawing.Size(72, 26);
            this.osize.TabIndex = 17;
            this.osize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.osize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.osize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.osize_MouseWheel);
            // 
            // Ticket
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(216, 85);
            this.Controls.Add(this.osize);
            this.Controls.Add(this.oprice);
            this.Controls.Add(this.marketbut);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.stopbut);
            this.Controls.Add(this.limitbut);
            this.Controls.Add(this.pricelabel);
            this.Controls.Add(this.sendbut);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.oprice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.osize)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton marketbut;
        private System.Windows.Forms.NumericUpDown oprice;
        private System.Windows.Forms.NumericUpDown osize;
    }
}