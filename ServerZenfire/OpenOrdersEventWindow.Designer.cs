namespace ZenFireDev
{
    partial class OpenOrdersEventWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenOrdersEventWindow));
            this.eventGrid = new System.Windows.Forms.DataGridView();
            this.colAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSymbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSide = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQtyFilled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLimitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coldStopPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAvgFillPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // eventGrid
            // 
            this.eventGrid.AllowUserToAddRows = false;
            this.eventGrid.AllowUserToDeleteRows = false;
            this.eventGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.eventGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAccount,
            this.colNumber,
            this.colStatus,
            this.colSymbol,
            this.colSide,
            this.colType,
            this.colQty,
            this.colQtyFilled,
            this.colLimitPrice,
            this.coldStopPrice,
            this.colAvgFillPrice,
            this.colOrderTime});
            this.eventGrid.Location = new System.Drawing.Point(13, 13);
            this.eventGrid.Name = "eventGrid";
            this.eventGrid.ReadOnly = true;
            this.eventGrid.RowHeadersVisible = false;
            this.eventGrid.Size = new System.Drawing.Size(1057, 271);
            this.eventGrid.TabIndex = 1;
            // 
            // colAccount
            // 
            this.colAccount.HeaderText = "Account";
            this.colAccount.Name = "colAccount";
            this.colAccount.ReadOnly = true;
            // 
            // colNumber
            // 
            this.colNumber.HeaderText = "Number";
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // colSymbol
            // 
            this.colSymbol.HeaderText = "Symbol";
            this.colSymbol.Name = "colSymbol";
            this.colSymbol.ReadOnly = true;
            // 
            // colSide
            // 
            this.colSide.HeaderText = "Side";
            this.colSide.Name = "colSide";
            this.colSide.ReadOnly = true;
            // 
            // colType
            // 
            this.colType.HeaderText = "Type";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colQty
            // 
            this.colQty.HeaderText = "Qty";
            this.colQty.Name = "colQty";
            this.colQty.ReadOnly = true;
            // 
            // colQtyFilled
            // 
            this.colQtyFilled.HeaderText = "QtyFilled";
            this.colQtyFilled.Name = "colQtyFilled";
            this.colQtyFilled.ReadOnly = true;
            // 
            // colLimitPrice
            // 
            this.colLimitPrice.HeaderText = "Limit Price";
            this.colLimitPrice.Name = "colLimitPrice";
            this.colLimitPrice.ReadOnly = true;
            // 
            // coldStopPrice
            // 
            this.coldStopPrice.HeaderText = "Stop Price";
            this.coldStopPrice.Name = "coldStopPrice";
            this.coldStopPrice.ReadOnly = true;
            // 
            // colAvgFillPrice
            // 
            this.colAvgFillPrice.HeaderText = "Avg Fill Price";
            this.colAvgFillPrice.Name = "colAvgFillPrice";
            this.colAvgFillPrice.ReadOnly = true;
            // 
            // colOrderTime
            // 
            this.colOrderTime.HeaderText = "Order Time";
            this.colOrderTime.Name = "colOrderTime";
            this.colOrderTime.ReadOnly = true;
            // 
            // OpenOrdersEventWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 296);
            this.Controls.Add(this.eventGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OpenOrdersEventWindow";
            this.Text = "Open Orders Event Watcher";
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView eventGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSymbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSide;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQtyFilled;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLimitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn coldStopPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvgFillPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderTime;
    }
}