namespace ZenFireDev
{
    partial class PositionDetailsEventWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PositionDetailsEventWindow));
            this.eventGrid = new System.Windows.Forms.DataGridView();
            this.colAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAvgFillPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOpenPL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSymbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClosedPL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLastTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalPL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // eventGrid
            // 
            this.eventGrid.AllowUserToAddRows = false;
            this.eventGrid.AllowUserToDeleteRows = false;
            this.eventGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAccount,
            this.colAvgFillPrice,
            this.colPosition,
            this.colOpenPL,
            this.colSymbol,
            this.colClosedPL,
            this.colLastTrade,
            this.colTotalPL});
            this.eventGrid.Location = new System.Drawing.Point(13, 13);
            this.eventGrid.Name = "eventGrid";
            this.eventGrid.ReadOnly = true;
            this.eventGrid.RowHeadersVisible = false;
            this.eventGrid.Size = new System.Drawing.Size(805, 231);
            this.eventGrid.TabIndex = 1;
            // 
            // colAccount
            // 
            this.colAccount.HeaderText = "Account";
            this.colAccount.Name = "colAccount";
            this.colAccount.ReadOnly = true;
            // 
            // colAvgFillPrice
            // 
            this.colAvgFillPrice.HeaderText = "Avg Fill Price";
            this.colAvgFillPrice.Name = "colAvgFillPrice";
            this.colAvgFillPrice.ReadOnly = true;
            // 
            // colPosition
            // 
            this.colPosition.HeaderText = "Position";
            this.colPosition.Name = "colPosition";
            this.colPosition.ReadOnly = true;
            // 
            // colOpenPL
            // 
            this.colOpenPL.HeaderText = "Open P&L";
            this.colOpenPL.Name = "colOpenPL";
            this.colOpenPL.ReadOnly = true;
            // 
            // colSymbol
            // 
            this.colSymbol.HeaderText = "Symbol";
            this.colSymbol.Name = "colSymbol";
            this.colSymbol.ReadOnly = true;
            // 
            // colClosedPL
            // 
            this.colClosedPL.HeaderText = "Closed P&L";
            this.colClosedPL.Name = "colClosedPL";
            this.colClosedPL.ReadOnly = true;
            // 
            // colLastTrade
            // 
            this.colLastTrade.HeaderText = "Last Trade";
            this.colLastTrade.Name = "colLastTrade";
            this.colLastTrade.ReadOnly = true;
            // 
            // colTotalPL
            // 
            this.colTotalPL.HeaderText = "Total P&L";
            this.colTotalPL.Name = "colTotalPL";
            this.colTotalPL.ReadOnly = true;
            // 
            // PositionDetailsEventWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 256);
            this.Controls.Add(this.eventGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PositionDetailsEventWindow";
            this.Text = "Position Details Event Watcher";
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView eventGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvgFillPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOpenPL;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSymbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClosedPL;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastTrade;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalPL;
    }
}