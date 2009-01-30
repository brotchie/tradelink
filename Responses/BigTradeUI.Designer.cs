namespace Responses
{
    partial class BigTradeUI
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
            this.dg = new System.Windows.Forms.DataGridView();
            this.butsell = new System.Windows.Forms.Button();
            this.butbuy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.SuspendLayout();
            // 
            // dg
            // 
            this.dg.AllowUserToAddRows = false;
            this.dg.AllowUserToDeleteRows = false;
            this.dg.AllowUserToOrderColumns = true;
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg.Location = new System.Drawing.Point(0, 0);
            this.dg.Name = "dg";
            this.dg.ReadOnly = true;
            this.dg.RowHeadersVisible = false;
            this.dg.RowTemplate.Height = 28;
            this.dg.ShowEditingIcon = false;
            this.dg.Size = new System.Drawing.Size(278, 248);
            this.dg.TabIndex = 0;
            // 
            // butsell
            // 
            this.butsell.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.butsell.Location = new System.Drawing.Point(0, 225);
            this.butsell.Name = "butsell";
            this.butsell.Size = new System.Drawing.Size(278, 23);
            this.butsell.TabIndex = 1;
            this.butsell.Text = "Sell";
            this.butsell.UseVisualStyleBackColor = true;
            this.butsell.Click += new System.EventHandler(this.butsell_Click);
            // 
            // butbuy
            // 
            this.butbuy.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.butbuy.Location = new System.Drawing.Point(0, 202);
            this.butbuy.Name = "butbuy";
            this.butbuy.Size = new System.Drawing.Size(278, 23);
            this.butbuy.TabIndex = 2;
            this.butbuy.Text = "Buy";
            this.butbuy.UseVisualStyleBackColor = true;
            this.butbuy.Click += new System.EventHandler(this.butbuy_Click);
            // 
            // BigTradeUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 248);
            this.Controls.Add(this.butbuy);
            this.Controls.Add(this.butsell);
            this.Controls.Add(this.dg);
            this.Name = "BigTradeUI";
            this.Text = "BigTradeUI";
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butsell;
        private System.Windows.Forms.Button butbuy;
        public System.Windows.Forms.DataGridView dg;
    }
}