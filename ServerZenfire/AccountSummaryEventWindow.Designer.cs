namespace ZenFireDev
{
    partial class AccountSummaryEventWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountSummaryEventWindow));
            this.eventAccountGrid = new System.Windows.Forms.DataGridView();
            this.Account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NetPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpenPL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClosedPL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalPL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EstimatedLV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CashOnHand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.eventAccountGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // eventAccountGrid
            // 
            this.eventAccountGrid.AllowUserToAddRows = false;
            this.eventAccountGrid.AllowUserToDeleteRows = false;
            this.eventAccountGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.eventAccountGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventAccountGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Account,
            this.NetPosition,
            this.OpenPL,
            this.ClosedPL,
            this.TotalPL,
            this.EstimatedLV,
            this.CashOnHand});
            this.eventAccountGrid.Location = new System.Drawing.Point(12, 13);
            this.eventAccountGrid.Name = "eventAccountGrid";
            this.eventAccountGrid.ReadOnly = true;
            this.eventAccountGrid.RowHeadersVisible = false;
            this.eventAccountGrid.Size = new System.Drawing.Size(700, 241);
            this.eventAccountGrid.TabIndex = 1;
            // 
            // Account
            // 
            this.Account.HeaderText = "Account";
            this.Account.Name = "Account";
            this.Account.ReadOnly = true;
            // 
            // NetPosition
            // 
            this.NetPosition.HeaderText = "Net Position";
            this.NetPosition.Name = "NetPosition";
            this.NetPosition.ReadOnly = true;
            // 
            // OpenPL
            // 
            this.OpenPL.HeaderText = "Open P&L";
            this.OpenPL.Name = "OpenPL";
            this.OpenPL.ReadOnly = true;
            // 
            // ClosedPL
            // 
            this.ClosedPL.HeaderText = "Closed P&L";
            this.ClosedPL.Name = "ClosedPL";
            this.ClosedPL.ReadOnly = true;
            // 
            // TotalPL
            // 
            this.TotalPL.HeaderText = "Total P&L";
            this.TotalPL.Name = "TotalPL";
            this.TotalPL.ReadOnly = true;
            // 
            // EstimatedLV
            // 
            this.EstimatedLV.HeaderText = "Estimated LV";
            this.EstimatedLV.Name = "EstimatedLV";
            this.EstimatedLV.ReadOnly = true;
            // 
            // CashOnHand
            // 
            this.CashOnHand.HeaderText = "Cash On Hand";
            this.CashOnHand.Name = "CashOnHand";
            this.CashOnHand.ReadOnly = true;
            // 
            // AccountSummaryEventWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 266);
            this.Controls.Add(this.eventAccountGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AccountSummaryEventWindow";
            this.Text = "Account Summary Event Watcher";
            ((System.ComponentModel.ISupportInitialize)(this.eventAccountGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView eventAccountGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Account;
        private System.Windows.Forms.DataGridViewTextBoxColumn NetPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpenPL;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClosedPL;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalPL;
        private System.Windows.Forms.DataGridViewTextBoxColumn EstimatedLV;
        private System.Windows.Forms.DataGridViewTextBoxColumn CashOnHand;

    }
}