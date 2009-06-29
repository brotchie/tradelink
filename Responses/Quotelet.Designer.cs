namespace Responses
{
    partial class Quotelet
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
            this._last = new System.Windows.Forms.TextBox();
            this._bid = new System.Windows.Forms.TextBox();
            this._ask = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._buy = new System.Windows.Forms.Button();
            this._sell = new System.Windows.Forms.Button();
            this._sym = new System.Windows.Forms.TextBox();
            this._pos = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._new = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _last
            // 
            this._last.Location = new System.Drawing.Point(12, 41);
            this._last.Name = "_last";
            this._last.ReadOnly = true;
            this._last.Size = new System.Drawing.Size(65, 26);
            this._last.TabIndex = 0;
            this._last.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _bid
            // 
            this._bid.Location = new System.Drawing.Point(83, 41);
            this._bid.Name = "_bid";
            this._bid.ReadOnly = true;
            this._bid.Size = new System.Drawing.Size(81, 26);
            this._bid.TabIndex = 1;
            this._bid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _ask
            // 
            this._ask.Location = new System.Drawing.Point(170, 41);
            this._ask.Name = "_ask";
            this._ask.ReadOnly = true;
            this._ask.Size = new System.Drawing.Size(73, 26);
            this._ask.TabIndex = 2;
            this._ask.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Last";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Bid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ask";
            // 
            // _buy
            // 
            this._buy.Location = new System.Drawing.Point(12, 84);
            this._buy.Name = "_buy";
            this._buy.Size = new System.Drawing.Size(75, 31);
            this._buy.TabIndex = 6;
            this._buy.Text = "buy";
            this._buy.UseVisualStyleBackColor = true;
            this._buy.Click += new System.EventHandler(this._buy_Click);
            // 
            // _sell
            // 
            this._sell.Location = new System.Drawing.Point(93, 84);
            this._sell.Name = "_sell";
            this._sell.Size = new System.Drawing.Size(75, 31);
            this._sell.TabIndex = 7;
            this._sell.Text = "sell";
            this._sell.UseVisualStyleBackColor = true;
            this._sell.Click += new System.EventHandler(this._sell_Click);
            // 
            // _sym
            // 
            this._sym.Location = new System.Drawing.Point(202, 86);
            this._sym.Name = "_sym";
            this._sym.Size = new System.Drawing.Size(62, 26);
            this._sym.TabIndex = 8;
            this._sym.Text = "IBM";
            // 
            // _pos
            // 
            this._pos.Location = new System.Drawing.Point(249, 41);
            this._pos.Name = "_pos";
            this._pos.ReadOnly = true;
            this._pos.Size = new System.Drawing.Size(73, 26);
            this._pos.TabIndex = 9;
            this._pos.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(266, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Pos";
            // 
            // _new
            // 
            this._new.Location = new System.Drawing.Point(270, 84);
            this._new.Name = "_new";
            this._new.Size = new System.Drawing.Size(52, 31);
            this._new.TabIndex = 11;
            this._new.Text = "ok";
            this._new.UseVisualStyleBackColor = true;
            this._new.Click += new System.EventHandler(this._new_Click);
            // 
            // Quotelet
            // 
            this.AcceptButton = this._new;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 127);
            this.Controls.Add(this._new);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._pos);
            this.Controls.Add(this._sym);
            this.Controls.Add(this._sell);
            this.Controls.Add(this._buy);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._ask);
            this.Controls.Add(this._bid);
            this.Controls.Add(this._last);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Quotelet";
            this.Text = "Quotelet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _last;
        private System.Windows.Forms.TextBox _bid;
        private System.Windows.Forms.TextBox _ask;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _buy;
        private System.Windows.Forms.Button _sell;
        private System.Windows.Forms.TextBox _sym;
        private System.Windows.Forms.TextBox _pos;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button _new;
    }
}